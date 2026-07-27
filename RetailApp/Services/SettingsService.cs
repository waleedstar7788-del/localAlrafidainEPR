using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _localConfigDirectory;
        private readonly string _localSettingsFilePath;

        public SettingsService()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _localConfigDirectory = Path.Combine(localAppData, "RetailApp", "Settings");
            _localSettingsFilePath = Path.Combine(_localConfigDirectory, "local_settings.json");

            if (!Directory.Exists(_localConfigDirectory))
            {
                Directory.CreateDirectory(_localConfigDirectory);
            }
        }

        public async Task<AppSettings> GetDbSettingsAsync()
        {
            using var dbContext = new AppDbContext();
            var settings = await dbContext.Settings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new AppSettings();
                dbContext.Settings.Add(settings);
                await dbContext.SaveChangesAsync();
            }
            return settings;
        }

        public async Task SaveDbSettingsAsync(AppSettings settings)
        {
            using var dbContext = new AppDbContext();
            var existing = await dbContext.Settings.FirstOrDefaultAsync();
            if (existing != null)
            {
                // Update properties
                dbContext.Entry(existing).CurrentValues.SetValues(settings);
            }
            else
            {
                dbContext.Settings.Add(settings);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<LocalSettings> GetLocalSettingsAsync()
        {
            if (!File.Exists(_localSettingsFilePath))
            {
                return new LocalSettings();
            }

            try
            {
                string json = await File.ReadAllTextAsync(_localSettingsFilePath);
                return JsonSerializer.Deserialize<LocalSettings>(json) ?? new LocalSettings();
            }
            catch
            {
                return new LocalSettings();
            }
        }

        public async Task SaveLocalSettingsAsync(LocalSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_localSettingsFilePath, json);
        }
    }
}
