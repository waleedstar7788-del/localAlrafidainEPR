using LiveChartsCore;
using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IStatisticsService
    {
        Task<List<StatItem>> GetDashboardStatsAsync();
        Task<List<ActivityItem>> GetRecentActivitiesAsync();
        Task<List<NotificationItem>> GetNotificationsAsync();
        Task<List<SystemStatusItem>> GetSystemStatusAsync();
        Task<ISeries[]> GetDailySalesChartAsync();
        Task<ISeries[]> GetMonthlySalesChartAsync();
        Task<ISeries[]> GetPurchasesVsSalesChartAsync();
        Task<ISeries[]> GetSalesByCategoryChartAsync();
    }
}
