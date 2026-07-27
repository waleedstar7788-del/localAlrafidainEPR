using System.Threading;
using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface ICompressionService
    {
        Task<string> CompressBackupAsync(string dbFilePath, string targetDirectory, BackupMetadata metadata, CancellationToken cancellationToken = default);
        Task<string> ExtractBackupAsync(string zipFilePath, string extractDirectory, CancellationToken cancellationToken = default);
    }
}
