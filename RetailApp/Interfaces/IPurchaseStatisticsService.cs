using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPurchaseStatisticsService
    {
        Task<int> GetTodayPurchasesCountAsync();
        Task<decimal> GetMonthlyPurchasesValueAsync();
        Task<decimal> GetOutstandingPurchasesValueAsync();
    }
}
