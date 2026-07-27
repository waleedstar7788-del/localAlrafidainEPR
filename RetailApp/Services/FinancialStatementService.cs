using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class FinancialStatementService : IFinancialStatementService
    {
        private readonly AppDbContext _context;

        public FinancialStatementService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<decimal> GetBalanceByCategoryAsync(AccountCategory category)
        {
            var accounts = await _context.Accounts.Where(a => a.Category == category).ToListAsync();
            decimal total = 0;
            
            // This is a simplified fetch; in production, you'd aggregate JournalEntryLines directly via SQL
            foreach (var acc in accounts)
            {
                var debits = await _context.JournalEntryLines
                    .Where(l => l.AccountId == acc.Id && l.JournalEntry.Status == JournalStatus.Posted)
                    .SumAsync(l => l.DebitAmount);
                    
                var credits = await _context.JournalEntryLines
                    .Where(l => l.AccountId == acc.Id && l.JournalEntry.Status == JournalStatus.Posted)
                    .SumAsync(l => l.CreditAmount);

                if (category == AccountCategory.Asset || category == AccountCategory.Expense)
                    total += (debits - credits);
                else
                    total += (credits - debits);
            }
            return total;
        }

        public async Task<decimal> GetTotalAssetsAsync() => await GetBalanceByCategoryAsync(AccountCategory.Asset);
        public async Task<decimal> GetTotalLiabilitiesAsync() => await GetBalanceByCategoryAsync(AccountCategory.Liability);
        public async Task<decimal> GetTotalEquityAsync() => await GetBalanceByCategoryAsync(AccountCategory.Equity);
        
        public async Task<decimal> GetNetIncomeAsync()
        {
            var revenues = await GetBalanceByCategoryAsync(AccountCategory.Revenue);
            var expenses = await GetBalanceByCategoryAsync(AccountCategory.Expense);
            return revenues - expenses;
        }
    }
}
