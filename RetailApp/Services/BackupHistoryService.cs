using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class BackupHistoryService : IBackupHistoryService
    {
        private readonly string _configDirectory;
        private readonly string _historyFilePath;

        public BackupHistoryService()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _configDirectory = Path.Combine(localAppData, "RetailApp", "BackupConfig");
            _historyFilePath = Path.Combine(_configDirectory, "backup_history.json");

            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
            }
        }

        public async Task<List<BackupHistoryItem>> GetHistoryAsync()
        {
            if (!File.Exists(_historyFilePath))
            {
                return new List<BackupHistoryItem>();
            }

            try
            {
                string json = await File.ReadAllTextAsync(_historyFilePath);
                return JsonSerializer.Deserialize<List<BackupHistoryItem>>(json) ?? new List<BackupHistoryItem>();
            }
            catch
            {
                return new List<BackupHistoryItem>();
            }
        }

        public async Task AddHistoryRecordAsync(BackupHistoryItem item)
        {
            var history = await GetHistoryAsync();
            history.Add(item);
            await SaveHistoryAsync(history);
        }

        public async Task UpdateHistoryRecordAsync(BackupHistoryItem item)
        {
            var history = await GetHistoryAsync();
            var index = history.FindIndex(h => h.Id == item.Id);
            if (index != -1)
            {
                history[index] = item;
                await SaveHistoryAsync(history);
            }
        }

        public async Task DeleteHistoryRecordAsync(string id)
        {
            var history = await GetHistoryAsync();
            var index = history.FindIndex(h => h.Id == id);
            if (index != -1)
            {
                history.RemoveAt(index);
                await SaveHistoryAsync(history);
            }
        }

        private async Task SaveHistoryAsync(List<BackupHistoryItem> history)
        {
            string json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_historyFilePath, json);
        }
    }
}
