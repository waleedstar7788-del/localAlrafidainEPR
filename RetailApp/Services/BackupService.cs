using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class BackupService : IBackupService
    {
        private readonly ICompressionService _compressionService;
        private readonly IBackupHistoryService _backupHistoryService;
        
        public event EventHandler<string>? ProgressChanged;

        public BackupService(ICompressionService compressionService, IBackupHistoryService backupHistoryService)
        {
            _compressionService = compressionService;
            _backupHistoryService = backupHistoryService;
        }

        public async Task<BackupHistoryItem> CreateBackupAsync(BackupType type, string? targetDirectory = null, CancellationToken cancellationToken = default)
        {
            var historyItem = new BackupHistoryItem
            {
                Date = DateTime.Now,
                User = Environment.UserName,
                BackupType = type,
                Status = BackupStatus.InProgress
            };

            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (string.IsNullOrWhiteSpace(targetDirectory))
                {
                    string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    targetDirectory = Path.Combine(documents, "ERP Backups");
                }

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                ReportProgress("جاري تجهيز بيانات النسخ الاحتياطي...");

                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string dbPath = Path.Combine(localAppData, "RetailApp", "app.db");

                if (!File.Exists(dbPath))
                {
                    throw new FileNotFoundException("لم يتم العثور على قاعدة البيانات.", dbPath);
                }

                long dbSize = new FileInfo(dbPath).Length;

                var metadata = new BackupMetadata
                {
                    AppVersion = typeof(BackupService).Assembly.GetName().Version?.ToString() ?? "1.0.0",
                    Timestamp = DateTime.Now,
                    BackupType = type,
                    DatabaseSizeBytes = dbSize,
                    MachineName = Environment.MachineName,
                    Username = Environment.UserName
                };

                ReportProgress("جاري ضغط الملفات...");
                string zipFilePath = await _compressionService.CompressBackupAsync(dbPath, targetDirectory, metadata, cancellationToken);

                stopwatch.Stop();
                
                historyItem.Duration = stopwatch.Elapsed;
                historyItem.SizeBytes = new FileInfo(zipFilePath).Length;
                historyItem.Location = zipFilePath;
                historyItem.Status = BackupStatus.Success;
                historyItem.RestoreAvailable = true;

                ReportProgress("تم إكمال النسخ الاحتياطي بنجاح.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                historyItem.Duration = stopwatch.Elapsed;
                historyItem.Status = BackupStatus.Failed;
                historyItem.ErrorMessage = ex.Message;
                ReportProgress($"فشل النسخ الاحتياطي: {ex.Message}");
            }
            finally
            {
                await _backupHistoryService.AddHistoryRecordAsync(historyItem);
            }

            return historyItem;
        }

        private void ReportProgress(string message)
        {
            ProgressChanged?.Invoke(this, message);
        }
    }
}
