namespace RetailApp.Interfaces
{
    public interface IDirectoryService
    {
        void InitializeDirectories();
        string GetDatabaseDirectory();
        string GetBackupsDirectory();
        string GetLogsDirectory();
        string GetExportsDirectory();
        string GetReportsDirectory();
        string GetTempDirectory();
        string GetLicensingDirectory();
    }
}
