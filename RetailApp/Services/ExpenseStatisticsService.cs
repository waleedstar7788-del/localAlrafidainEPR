using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class ExpenseStatisticsService : IExpenseStatisticsService
    {
        private readonly AppDbContext _context;

        public ExpenseStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTodaysExpensesAsync()
        {
            var today = DateTime.Today;
            return await _context.ExpenseEntries
                .Where(e => e.ExpenseDate.Date == today && e.Status != Models.FinancialTransactionStatus.Cancelled)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> GetMonthlyExpensesAsync()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return await _context.ExpenseEntries
                .Where(e => e.ExpenseDate >= startOfMonth && e.Status != Models.FinancialTransactionStatus.Cancelled)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> GetTodaysIncomeAsync()
        {
            var today = DateTime.Today;
            return await _context.IncomeEntries
                .Where(i => i.IncomeDate.Date == today && i.Status != Models.FinancialTransactionStatus.Cancelled)
                .SumAsync(i => i.Amount);
        }

        public async Task<decimal> GetMonthlyIncomeAsync()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return await _context.IncomeEntries
                .Where(i => i.IncomeDate >= startOfMonth && i.Status != Models.FinancialTransactionStatus.Cancelled)
                .SumAsync(i => i.Amount);
        }
    }
}
