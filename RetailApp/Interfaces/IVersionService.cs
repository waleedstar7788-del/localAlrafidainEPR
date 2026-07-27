namespace RetailApp.Interfaces
{
    public interface IVersionService
    {
        string GetCurrentVersion();
        string GetBuildNumber();
        string GetReleaseDate();
    }
}
