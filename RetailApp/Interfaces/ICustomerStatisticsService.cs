using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ICustomerStatisticsService
    {
        Task<int> GetTotalCustomersAsync();
        Task<int> GetNewCustomersThisMonthAsync();
        Task<int> GetVipCustomersCountAsync();
        Task<int> GetCustomersWithDebtCountAsync();
    }
}
