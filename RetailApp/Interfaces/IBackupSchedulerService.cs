using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface IBackupSchedulerService
    {
        Task<BackupScheduleConfig> GetScheduleConfigAsync();
        Task SaveScheduleConfigAsync(BackupScheduleConfig config);
        Task RunScheduledBackupsAsync();
        Task EnforceRetentionPolicyAsync();
    }
}
