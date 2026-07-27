using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class IntegrityVerificationService : IIntegrityVerificationService
    {
        public Task<bool> VerifyZipIntegrityAsync(string zipFilePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            // Try reading a few bytes to ensure integrity
                            using (Stream stream = entry.Open())
                            {
                                stream.Read(new byte[1], 0, 1);
                            }
                        }
                    }
                    return true;
                }
                catch (InvalidDataException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public async Task<bool> VerifyDatabaseIntegrityAsync(string dbFilePath)
        {
            try
            {
                string connectionString = $"Data Source={dbFilePath};Mode=ReadOnly";
                using (var connection = new SqliteConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "PRAGMA integrity_check;";
                        var result = await command.ExecuteScalarAsync();
                        return result != null && result.ToString() == "ok";
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerifyVersionCompatibilityAsync(string zipFilePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                    {
                        var metadataEntry = archive.GetEntry("metadata.json");
                        if (metadataEntry == null) return false;

                        using (var stream = metadataEntry.Open())
                        using (var reader = new StreamReader(stream))
                        {
                            string json = reader.ReadToEnd();
                            var metadata = JsonSerializer.Deserialize<BackupMetadata>(json);
                            if (metadata == null || string.IsNullOrEmpty(metadata.AppVersion)) return false;
                            
                            // For simplicity, accepting any valid version format
                            // In real scenarios, check compatibility matrices
                            return Version.TryParse(metadata.AppVersion, out _);
                        }
                    }
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
