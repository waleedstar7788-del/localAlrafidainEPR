using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class BackupBackgroundWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BackupBackgroundWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var schedulerService = scope.ServiceProvider.GetRequiredService<IBackupSchedulerService>();
                        await schedulerService.RunScheduledBackupsAsync();
                    }
                }
                catch (Exception)
                {
                    // Ignore exceptions to prevent the background service from crashing.
                }

                // Check every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
