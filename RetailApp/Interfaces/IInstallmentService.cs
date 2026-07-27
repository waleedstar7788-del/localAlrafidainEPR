using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IInstallmentService
    {
        Task<List<InstallmentContract>> GetContractsAsync(int pageNumber, int pageSize);
        Task<InstallmentContract?> GetContractByIdAsync(int id);
        Task<InstallmentContract> GenerateContractAsync(InstallmentContract contract);
        Task CancelContractAsync(int contractId);
    }
}
