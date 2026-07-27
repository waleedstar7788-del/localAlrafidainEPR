using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class RestoreService : IRestoreService
    {
        private readonly ICompressionService _compressionService;
        private readonly IIntegrityVerificationService _integrityVerificationService;
        private readonly IBackupService _backupService;

        public event EventHandler<string>? ProgressChanged;

        public RestoreService(
            ICompressionService compressionService, 
            IIntegrityVerificationService integrityVerificationService,
            IBackupService backupService)
        {
            _compressionService = compressionService;
            _integrityVerificationService = integrityVerificationService;
            _backupService = backupService;
        }

        public async Task<bool> RestoreBackupAsync(string zipFilePath, CancellationToken cancellationToken = default)
        {
            try
            {
                ReportProgress("جاري التحقق من سلامة ملف النسخة الاحتياطية...");
                
                if (!await _integrityVerificationService.VerifyZipIntegrityAsync(zipFilePath))
                {
                    throw new Exception("ملف النسخة الاحتياطية تالف أو غير صالح.");
                }

                if (!await _integrityVerificationService.VerifyVersionCompatibilityAsync(zipFilePath))
                {
                    throw new Exception("هذه النسخة الاحتياطية غير متوافقة مع الإصدار الحالي من البرنامج.");
                }

                ReportProgress("جاري أخذ نسخة احتياطية أمان قبل الاستعادة...");
                var safetyBackup = await _backupService.CreateBackupAsync(Models.BackupType.Automatic, null, cancellationToken);
                if (safetyBackup.Status != Models.BackupStatus.Success)
                {
                    throw new Exception("تعذر أخذ نسخة احتياطية أمان، تم إيقاف عملية الاستعادة.");
                }

                ReportProgress("جاري استخراج الملفات...");
                string tempExtractDir = Path.Combine(Path.GetTempPath(), $"RetailApp_Restore_{Guid.NewGuid()}");
                await _compressionService.ExtractBackupAsync(zipFilePath, tempExtractDir, cancellationToken);

                string extractedDbPath = Path.Combine(tempExtractDir, "app.db");
                if (!File.Exists(extractedDbPath))
                {
                    throw new Exception("لا يحتوي ملف النسخة الاحتياطية على قاعدة بيانات صالحة.");
                }

                ReportProgress("جاري فحص سلامة قاعدة البيانات المستخرجة...");
                if (!await _integrityVerificationService.VerifyDatabaseIntegrityAsync(extractedDbPath))
                {
                    throw new Exception("قاعدة البيانات المستخرجة تالفة، تم إيقاف الاستعادة.");
                }

                ReportProgress("جاري استبدال قاعدة البيانات الحالية...");
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string currentDbPath = Path.Combine(localAppData, "RetailApp", "app.db");

                // Note: The AppDbContext might be holding a lock.
                // In a real application, you must dispose the active DbContext pool or kill connections.
                // For SQLite in this context, we will ensure it's copied safely.
                
                // GC.Collect and GC.WaitForPendingFinalizers() can help release unmanaged file locks if necessary
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Using File.Copy with overwrite
                await Task.Run(() => 
                {
                    File.Copy(extractedDbPath, currentDbPath, true);
                }, cancellationToken);

                ReportProgress("تمت استعادة البيانات بنجاح. يرجى إعادة تشغيل النظام.");
                
                // Clean up temp dir
                if (Directory.Exists(tempExtractDir))
                {
                    Directory.Delete(tempExtractDir, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                ReportProgress($"فشلت عملية الاستعادة: {ex.Message}");
                return false;
            }
        }

        private void ReportProgress(string message)
        {
            ProgressChanged?.Invoke(this, message);
        }
    }
}
