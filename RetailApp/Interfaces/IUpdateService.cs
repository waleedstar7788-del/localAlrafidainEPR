using System;
using System.Threading.Tasks;
using RetailApp.Models;

namespace RetailApp.Interfaces
{
    public interface IUpdateService
    {
        Task<UpdateInfo?> CheckForUpdatesAsync();
        Task<bool> DownloadUpdateAsync(string url, IProgress<double>? progress = null);
    }
}
