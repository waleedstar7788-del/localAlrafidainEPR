using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string moduleName, string action, string description);
        Task<List<AuditLog>> GetLogsAsync(int take = 100);
    }
}
