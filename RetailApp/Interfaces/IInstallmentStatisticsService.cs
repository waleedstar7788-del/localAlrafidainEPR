using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IInstallmentStatisticsService
    {
        Task<decimal> GetTotalOutstandingDebtAsync();
        Task<decimal> GetTodaysCollectionsAsync();
        Task<decimal> GetMonthlyCollectionsAsync();
        Task<int> GetLateInstallmentsCountAsync();
    }
}
