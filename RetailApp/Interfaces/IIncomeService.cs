using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IIncomeService
    {
        Task<IncomeEntry> AddIncomeAsync(IncomeEntry income);
        Task<List<IncomeEntry>> GetIncomesAsync(int page = 1, int pageSize = 100);
        Task CancelIncomeAsync(int incomeId);
    }
}
