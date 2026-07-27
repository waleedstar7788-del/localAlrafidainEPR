using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface ILicenseService
    {
        Task<LicenseData?> GetCurrentLicenseAsync();
        Task SaveLicenseAsync(LicenseData license);
        Task<LicenseStatus> ValidateCurrentLicenseAsync();
        Task StartTrialAsync();
        Task ClearLicenseAsync();
    }
}
