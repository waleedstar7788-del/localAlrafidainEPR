using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace RetailApp.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuditLogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task LogActionAsync(string moduleName, string action, string description)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var authService = scope.ServiceProvider.GetService<IAuthenticationService>();

            var log = new AuditLog
            {
                Timestamp = DateTime.Now,
                ModuleName = moduleName,
                Action = action,
                Description = description,
                UserId = authService?.CurrentUser?.Id,
                Username = authService?.CurrentUser?.Username ?? "System",
                ComputerName = Environment.MachineName
            };

            context.AuditLogs.Add(log);
            await context.SaveChangesAsync();
        }

        public async Task<List<AuditLog>> GetLogsAsync(int take = 100)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            return await context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(l => l.Timestamp)
                .Take(take)
                .ToListAsync();
        }
    }
}
