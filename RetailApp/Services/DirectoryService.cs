using System;
using System.IO;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly string _baseDirectory;

        public DirectoryService()
        {
            _baseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RetailApp");
            // Ensure base directory exists
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        public void InitializeDirectories()
        {
            var directories = new[]
            {
                GetDatabaseDirectory(),
                GetBackupsDirectory(),
                GetLogsDirectory(),
                GetExportsDirectory(),
                GetReportsDirectory(),
                GetTempDirectory(),
                GetLicensingDirectory()
            };

            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        public string GetDatabaseDirectory() => Path.Combine(_baseDirectory, "Database");
        public string GetBackupsDirectory() => Path.Combine(_baseDirectory, "Backups");
        public string GetLogsDirectory() => Path.Combine(_baseDirectory, "Logs");
        public string GetExportsDirectory() => Path.Combine(_baseDirectory, "Exports");
        public string GetReportsDirectory() => Path.Combine(_baseDirectory, "Reports");
        public string GetTempDirectory()
        {
            var tempPath = Path.Combine(_baseDirectory, "Temp");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }
        public string GetLicensingDirectory() => Path.Combine(_baseDirectory, "Licensing");
    }
}
