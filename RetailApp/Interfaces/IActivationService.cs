using System;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IActivationService
    {
        event Action? OnLicenseChanged;
        Task<(bool Success, string Message)> ActivateOfflineAsync(string licenseInput);
        Task<bool> DeactivateAsync();
        Task<string> ExportMachineRequestAsync();
    }
}
