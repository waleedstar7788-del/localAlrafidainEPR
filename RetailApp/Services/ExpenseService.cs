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
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;
        private readonly IJournalService _journalService;

        public ExpenseService(AppDbContext context, IJournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        public async Task<List<ExpenseEntry>> GetExpensesAsync(int page = 1, int pageSize = 100)
        {
            return await _context.ExpenseEntries
                .Include(e => e.Account)
                .OrderByDescending(e => e.ExpenseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ExpenseEntry> AddExpenseAsync(ExpenseEntry expense)
        {
            expense.ExpenseNumber = "EXP-" + DateTime.Now.Ticks.ToString().Substring(10);
            expense.Status = FinancialTransactionStatus.Approved; // Auto approve for simplicity

            _context.ExpenseEntries.Add(expense);
            await _context.SaveChangesAsync();

            // Journal Entry Integration
            // Debit: Selected Expense Account (e.g. 5200)
            // Credit: Cash (1100) or Bank (1200) based on PaymentMethod
            
            var creditAccount = expense.PaymentMethod == FinancialPaymentMethod.Cash ? "1100" : "1200";
            
            // Get the account number from DB just to be safe
            var expenseAccount = await _context.Accounts.FindAsync(expense.AccountId);
            if (expenseAccount != null)
            {
                var journalLines = new List<(string AccountNumber, decimal Debit, decimal Credit)>
                {
                    (expenseAccount.AccountNumber, expense.Amount, 0),
                    (creditAccount, 0, expense.Amount)
                };

                await _journalService.GenerateAutomaticEntryAsync(expense.ExpenseNumber, $"مصروفات: {expense.Title}", journalLines);
            }

            return expense;
        }

        public async Task ApproveExpenseAsync(int expenseId, string approvedBy)
        {
            var exp = await _context.ExpenseEntries.FindAsync(expenseId);
            if (exp != null && exp.Status == FinancialTransactionStatus.Pending)
            {
                exp.Status = FinancialTransactionStatus.Approved;
                exp.ApprovedBy = approvedBy;
                await _context.SaveChangesAsync();
                // Depending on rules, journal entry could be deferred until approval.
            }
        }

        public async Task CancelExpenseAsync(int expenseId)
        {
            var exp = await _context.ExpenseEntries.FindAsync(expenseId);
            if (exp != null)
            {
                exp.Status = FinancialTransactionStatus.Cancelled;
                await _context.SaveChangesAsync();
                // Should theoretically create a reversing journal entry here.
            }
        }
    }
}
