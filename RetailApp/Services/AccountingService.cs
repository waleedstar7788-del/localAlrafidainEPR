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
    public class AccountingService : IAccountingService
    {
        private readonly AppDbContext _context;

        public AccountingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedDefaultAccountsAsync()
        {
            if (await _context.Accounts.AnyAsync())
                return;

            var defaultAccounts = new List<Account>
            {
                new Account { AccountNumber = "1100", Name = "الصندوق (Cash)", Category = AccountCategory.Asset, IsSystemAccount = true },
                new Account { AccountNumber = "1200", Name = "البنك (Bank)", Category = AccountCategory.Asset, IsSystemAccount = true },
                new Account { AccountNumber = "1300", Name = "العملاء (Accounts Receivable)", Category = AccountCategory.Asset, IsSystemAccount = true },
                new Account { AccountNumber = "1400", Name = "المخزون (Inventory)", Category = AccountCategory.Asset, IsSystemAccount = true },
                
                new Account { AccountNumber = "2100", Name = "الموردون (Accounts Payable)", Category = AccountCategory.Liability, IsSystemAccount = true },
                
                new Account { AccountNumber = "3100", Name = "رأس المال (Capital)", Category = AccountCategory.Equity, IsSystemAccount = true },
                
                new Account { AccountNumber = "4100", Name = "إيرادات المبيعات (Sales Revenue)", Category = AccountCategory.Revenue, IsSystemAccount = true },
                new Account { AccountNumber = "4200", Name = "خصومات مكتسبة (Discounts Received)", Category = AccountCategory.Revenue, IsSystemAccount = true },
                
                new Account { AccountNumber = "5100", Name = "تكلفة البضاعة المباعة (COGS)", Category = AccountCategory.Expense, IsSystemAccount = true },
                new Account { AccountNumber = "5200", Name = "مصروفات عامة (General Expenses)", Category = AccountCategory.Expense, IsSystemAccount = true },
                new Account { AccountNumber = "5300", Name = "خصومات مسموح بها (Discounts Given)", Category = AccountCategory.Expense, IsSystemAccount = true }
            };

            _context.Accounts.AddRange(defaultAccounts);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts.OrderBy(a => a.AccountNumber).ToListAsync();
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                if (account.IsSystemAccount)
                    throw new InvalidOperationException("Cannot delete a system default account.");

                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetAccountBalanceAsync(int accountId)
        {
            var lines = await _context.JournalEntryLines
                .Include(l => l.JournalEntry)
                .Where(l => l.AccountId == accountId && l.JournalEntry.Status == JournalStatus.Posted)
                .ToListAsync();

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return 0;

            decimal totalDebits = lines.Sum(l => l.DebitAmount);
            decimal totalCredits = lines.Sum(l => l.CreditAmount);

            // Normal balance rules
            if (account.Category == AccountCategory.Asset || account.Category == AccountCategory.Expense)
            {
                return totalDebits - totalCredits;
            }
            else
            {
                return totalCredits - totalDebits;
            }
        }
    }
}
