using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface ILicenseValidationService
    {
        bool ValidateSignature(LicenseData data);
        string GenerateLicenseKey();
        string GenerateSignature(LicenseData data);
    }
}
