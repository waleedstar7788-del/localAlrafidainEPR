using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class CompressionService : ICompressionService
    {
        public async Task<string> CompressBackupAsync(string dbFilePath, string targetDirectory, BackupMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string zipFileName = $"RetailApp_Backup_{timestamp}.zip";
            string zipFilePath = Path.Combine(targetDirectory, zipFileName);

            string tempDir = Path.Combine(Path.GetTempPath(), $"RetailApp_TempBackup_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDir);

            try
            {
                // Copy DB to temp dir
                string tempDbPath = Path.Combine(tempDir, "app.db");
                File.Copy(dbFilePath, tempDbPath, true);

                // Write metadata.json to temp dir
                string metadataPath = Path.Combine(tempDir, "metadata.json");
                string json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(metadataPath, json, cancellationToken);

                // Create Zip
                if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
                
                await Task.Run(() => ZipFile.CreateFromDirectory(tempDir, zipFilePath, CompressionLevel.Optimal, false), cancellationToken);

                return zipFilePath;
            }
            finally
            {
                // Clean up temp dir
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        public async Task<string> ExtractBackupAsync(string zipFilePath, string extractDirectory, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("Backup file not found.", zipFilePath);
            }

            if (Directory.Exists(extractDirectory))
            {
                Directory.Delete(extractDirectory, true);
            }
            
            Directory.CreateDirectory(extractDirectory);

            await Task.Run(() => ZipFile.ExtractToDirectory(zipFilePath, extractDirectory, true), cancellationToken);

            return extractDirectory;
        }
    }
}
