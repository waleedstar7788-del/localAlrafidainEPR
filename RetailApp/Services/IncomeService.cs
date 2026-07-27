using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly AppDbContext _context;
        private readonly IJournalService _journalService;

        public IncomeService(AppDbContext context, IJournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        public async Task<List<IncomeEntry>> GetIncomesAsync(int page = 1, int pageSize = 100)
        {
            return await _context.IncomeEntries
                .Include(i => i.Account)
                .OrderByDescending(i => i.IncomeDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IncomeEntry> AddIncomeAsync(IncomeEntry income)
        {
            income.IncomeNumber = "INC-" + DateTime.Now.Ticks.ToString().Substring(10);
            income.Status = FinancialTransactionStatus.Approved;

            _context.IncomeEntries.Add(income);
            await _context.SaveChangesAsync();

            // Journal Entry Integration
            // Debit: Cash (1100) or Bank (1200) based on PaymentMethod
            // Credit: Selected Revenue Account (e.g. 4300)
            
            var debitAccount = income.PaymentMethod == FinancialPaymentMethod.Cash ? "1100" : "1200";
            var incomeAccount = await _context.Accounts.FindAsync(income.AccountId);
            if (incomeAccount != null)
            {
                var journalLines = new List<(string AccountNumber, decimal Debit, decimal Credit)>
                {
                    (debitAccount, income.Amount, 0),
                    (incomeAccount.AccountNumber, 0, income.Amount)
                };

                await _journalService.GenerateAutomaticEntryAsync(income.IncomeNumber, $"إيرادات خارجية: {income.Title}", journalLines);
            }

            return income;
        }

        public async Task CancelIncomeAsync(int incomeId)
        {
            var inc = await _context.IncomeEntries.FindAsync(incomeId);
            if (inc != null)
            {
                inc.Status = FinancialTransactionStatus.Cancelled;
                await _context.SaveChangesAsync();
            }
        }
    }
}
