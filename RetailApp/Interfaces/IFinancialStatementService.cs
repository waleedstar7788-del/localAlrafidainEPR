using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IFinancialStatementService
    {
        Task<decimal> GetTotalAssetsAsync();
        Task<decimal> GetTotalLiabilitiesAsync();
        Task<decimal> GetTotalEquityAsync();
        Task<decimal> GetNetIncomeAsync();
    }
}
