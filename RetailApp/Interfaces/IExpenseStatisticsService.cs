using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IExpenseStatisticsService
    {
        Task<decimal> GetTodaysExpensesAsync();
        Task<decimal> GetMonthlyExpensesAsync();
        Task<decimal> GetTodaysIncomeAsync();
        Task<decimal> GetMonthlyIncomeAsync();
    }
}
