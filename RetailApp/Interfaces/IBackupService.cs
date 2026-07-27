using System;
using System.Threading;
using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface IBackupService
    {
        event EventHandler<string>? ProgressChanged;
        Task<BackupHistoryItem> CreateBackupAsync(BackupType type, string? targetDirectory = null, CancellationToken cancellationToken = default);
    }
}
