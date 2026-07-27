using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IIntegrityVerificationService
    {
        Task<bool> VerifyZipIntegrityAsync(string zipFilePath);
        Task<bool> VerifyDatabaseIntegrityAsync(string dbFilePath);
        Task<bool> VerifyVersionCompatibilityAsync(string zipFilePath);
    }
}
