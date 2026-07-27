using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IReturnStatisticsService
    {
        Task<decimal> GetMonthlySalesReturnTotalAsync();
        Task<decimal> GetMonthlyPurchaseReturnTotalAsync();
        Task<int> GetTodayReturnsCountAsync();
    }
}
