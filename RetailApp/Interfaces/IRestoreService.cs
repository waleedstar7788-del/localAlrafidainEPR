using System;
using System.Threading;
using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface IRestoreService
    {
        event EventHandler<string> ProgressChanged;
        Task<bool> RestoreBackupAsync(string zipFilePath, CancellationToken cancellationToken = default);
    }
}
