using System.Collections.Generic;
using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface IBackupHistoryService
    {
        Task<List<BackupHistoryItem>> GetHistoryAsync();
        Task AddHistoryRecordAsync(BackupHistoryItem item);
        Task DeleteHistoryRecordAsync(string id);
        Task UpdateHistoryRecordAsync(BackupHistoryItem item);
    }
}
