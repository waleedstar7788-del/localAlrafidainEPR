using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IJournalService
    {
        Task<JournalEntry> CreateJournalAsync(JournalEntry journal);
        Task PostJournalAsync(int journalId);
        Task<List<JournalEntry>> GetAllJournalsAsync();
        Task GenerateAutomaticEntryAsync(string reference, string description, List<(string AccountNumber, decimal Debit, decimal Credit)> lines);
    }
}
