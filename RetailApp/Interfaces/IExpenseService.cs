using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseEntry> AddExpenseAsync(ExpenseEntry expense);
        Task<List<ExpenseEntry>> GetExpensesAsync(int page = 1, int pageSize = 100);
        Task ApproveExpenseAsync(int expenseId, string approvedBy);
        Task CancelExpenseAsync(int expenseId);
    }
}
