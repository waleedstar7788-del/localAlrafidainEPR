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
    public class JournalService : IJournalService
    {
        private readonly AppDbContext _context;

        public JournalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<JournalEntry>> GetAllJournalsAsync()
        {
            return await _context.JournalEntries
                .Include(j => j.Lines)
                .ThenInclude(l => l.Account)
                .OrderByDescending(j => j.Date)
                .ToListAsync();
        }

        public async Task<JournalEntry> CreateJournalAsync(JournalEntry journal)
        {
            decimal totalDebits = journal.Lines.Sum(l => l.DebitAmount);
            decimal totalCredits = journal.Lines.Sum(l => l.CreditAmount);

            if (totalDebits != totalCredits)
                throw new InvalidOperationException($"Journal is unbalanced. Debits: {totalDebits}, Credits: {totalCredits}");

            if (totalDebits == 0)
                throw new InvalidOperationException("Journal must have a non-zero amount.");

            journal.JournalNumber = "JRN-" + DateTime.Now.Ticks.ToString().Substring(10);
            journal.CreatedDate = DateTime.Now;

            _context.JournalEntries.Add(journal);
            await _context.SaveChangesAsync();

            return journal;
        }

        public async Task PostJournalAsync(int journalId)
        {
            var journal = await _context.JournalEntries.FindAsync(journalId);
            if (journal != null && journal.Status == JournalStatus.Draft)
            {
                journal.Status = JournalStatus.Posted;
                await _context.SaveChangesAsync();
            }
        }

        public async Task GenerateAutomaticEntryAsync(string reference, string description, List<(string AccountNumber, decimal Debit, decimal Credit)> lineData)
        {
            var journal = new JournalEntry
            {
                Date = DateTime.Now,
                ReferenceNumber = reference,
                Description = description,
                Status = JournalStatus.Posted, // Auto entries post immediately
                CreatedBy = "System"
            };

            foreach (var data in lineData)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == data.AccountNumber);
                if (account == null) continue; // Skip if account not found (e.g., seeding failed)

                if (data.Debit > 0 || data.Credit > 0)
                {
                    journal.Lines.Add(new JournalEntryLine
                    {
                        AccountId = account.Id,
                        DebitAmount = data.Debit,
                        CreditAmount = data.Credit,
                        Description = description
                    });
                }
            }

            if (journal.Lines.Any())
            {
                await CreateJournalAsync(journal);
            }
        }
    }
}
