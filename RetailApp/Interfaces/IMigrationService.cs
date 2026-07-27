using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IMigrationService
    {
        Task MigrateDatabaseAsync();
        Task SeedDefaultDataAsync(string adminUsername, string adminPassword, string companyName);
    }
}
