using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface ISettingsService
    {
        Task<AppSettings> GetDbSettingsAsync();
        Task SaveDbSettingsAsync(AppSettings settings);

        Task<LocalSettings> GetLocalSettingsAsync();
        Task SaveLocalSettingsAsync(LocalSettings settings);
    }
}
