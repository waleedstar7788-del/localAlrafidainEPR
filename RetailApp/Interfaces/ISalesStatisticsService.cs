using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ISalesStatisticsService
    {
        Task<decimal> GetTodaySalesTotalAsync();
        Task<decimal> GetMonthlySalesTotalAsync();
        Task<decimal> GetTodayProfitAsync();
        Task<decimal> GetMonthlyProfitAsync();
    }
}
