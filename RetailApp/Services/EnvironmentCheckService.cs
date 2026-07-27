using System;
using System.IO;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class EnvironmentCheckService : IEnvironmentCheckService
    {
        private readonly IDirectoryService _directoryService;

        public EnvironmentCheckService(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        public bool ValidateEnvironment(out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Ensure temp directory exists
                var tempDir = _directoryService.GetTempDirectory();
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                // Check Write Permissions
                var testFile = Path.Combine(tempDir, "test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);

                // Check Disk Space (Require at least 500MB)
                var driveInfo = new DriveInfo(Path.GetPathRoot(_directoryService.GetTempDirectory()) ?? "C:\\");
                if (driveInfo.AvailableFreeSpace < 500 * 1024 * 1024)
                {
                    errorMessage = "المساحة المتوفرة غير كافية. يرجى توفير 500 ميغابايت على الأقل.";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"حدث خطأ في صلاحيات القراءة والكتابة: {ex.Message}";
                return false;
            }
        }
    }
}
