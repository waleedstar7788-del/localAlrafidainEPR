using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class BackupSchedulerService : IBackupSchedulerService
    {
        private readonly string _configDirectory;
        private readonly string _scheduleFilePath;
        private readonly IServiceProvider _serviceProvider;

        public BackupSchedulerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _configDirectory = Path.Combine(localAppData, "RetailApp", "BackupConfig");
            _scheduleFilePath = Path.Combine(_configDirectory, "backup_schedule.json");

            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }
        }

        public async Task<BackupScheduleConfig> GetScheduleConfigAsync()
        {
            if (!File.Exists(_scheduleFilePath))
            {
                return new BackupScheduleConfig();
            }

            try
            {
                string json = await File.ReadAllTextAsync(_scheduleFilePath);
                return JsonSerializer.Deserialize<BackupScheduleConfig>(json) ?? new BackupScheduleConfig();
            }
            catch
            {
                return new BackupScheduleConfig();
            }
        }

        public async Task SaveScheduleConfigAsync(BackupScheduleConfig config)
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_scheduleFilePath, json);
        }

        public async Task RunScheduledBackupsAsync()
        {
            var config = await GetScheduleConfigAsync();
            if (config.Frequency == BackupFrequency.None) return;

            bool shouldRun = false;
            var now = DateTime.Now;

            if (config.LastRun.HasValue)
            {
                var timeSinceLastRun = now - config.LastRun.Value;
                switch (config.Frequency)
                {
                    case BackupFrequency.Daily:
                        shouldRun = timeSinceLastRun.TotalHours >= 24 || now.Date > config.LastRun.Value.Date;
                        break;
                    case BackupFrequency.Weekly:
                        shouldRun = timeSinceLastRun.TotalDays >= 7 || (now.DayOfWeek == config.ScheduledDayOfWeek && now.Date > config.LastRun.Value.Date);
                        break;
                    case BackupFrequency.Monthly:
                        shouldRun = (now.Month != config.LastRun.Value.Month) && now.Day >= config.ScheduledDayOfMonth;
                        break;
                }
            }
            else
            {
                shouldRun = true;
            }

            if (shouldRun)
            {
                // To avoid circular dependency during DI construction, resolve IBackupService dynamically
                using var scope = _serviceProvider.CreateScope();
                var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                
                var historyItem = await backupService.CreateBackupAsync(BackupType.Scheduled);
                
                if (historyItem.Status == BackupStatus.Success)
                {
                    config.LastRun = DateTime.Now;
                    await SaveScheduleConfigAsync(config);
                    await EnforceRetentionPolicyAsync();
                }
            }
        }

        public async Task EnforceRetentionPolicyAsync()
        {
            var config = await GetScheduleConfigAsync();
            if (config.MaxRetainedBackups <= 0) return;

            using var scope = _serviceProvider.CreateScope();
            var historyService = scope.ServiceProvider.GetRequiredService<IBackupHistoryService>();
            
            var history = await historyService.GetHistoryAsync();
            
            var automaticBackups = history
                .Where(h => h.BackupType == BackupType.Scheduled || h.BackupType == BackupType.Automatic || h.BackupType == BackupType.OnExit || h.BackupType == BackupType.OnStartup)
                .OrderByDescending(h => h.Date)
                .ToList();

            if (automaticBackups.Count > config.MaxRetainedBackups)
            {
                var toDelete = automaticBackups.Skip(config.MaxRetainedBackups).ToList();
                foreach (var item in toDelete)
                {
                    try
                    {
                        if (File.Exists(item.Location))
                        {
                            File.Delete(item.Location);
                        }
                        await historyService.DeleteHistoryRecordAsync(item.Id);
                    }
                    catch (Exception)
                    {
                        // Ignore deletion errors, log them if logging is configured
                    }
                }
            }
        }
    }
}
