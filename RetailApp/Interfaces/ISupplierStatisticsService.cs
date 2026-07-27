using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ISupplierStatisticsService
    {
        Task<int> GetTotalSuppliersAsync();
        Task<int> GetActiveSuppliersAsync();
        Task<int> GetSuppliersWithOutstandingBalanceAsync();
        Task<int> GetNewSuppliersThisMonthAsync();
    }
}
