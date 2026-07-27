using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.ViewModels
{
    public partial class UpdateViewModel : BaseViewModel
    {
        private readonly IUpdateService _updateService;
        private readonly IVersionService _versionService;

        [ObservableProperty]
        private string _currentVersion = string.Empty;

        [ObservableProperty]
        private UpdateInfo? _availableUpdate;

        [ObservableProperty]
        private bool _isUpdateAvailable;

        [ObservableProperty]
        private bool _isChecking;

        [ObservableProperty]
        private bool _isDownloading;

        [ObservableProperty]
        private double _downloadProgress;

        [ObservableProperty]
        private string _progressText = "0%";

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        public ICommand CheckForUpdatesCommand { get; }
        public ICommand DownloadUpdateCommand { get; }

        public UpdateViewModel(IUpdateService updateService, IVersionService versionService)
        {
            _updateService = updateService;
            _versionService = versionService;

            CurrentVersion = _versionService.GetCurrentVersion();

            CheckForUpdatesCommand = new AsyncRelayCommand(CheckForUpdatesAsync);
            DownloadUpdateCommand = new AsyncRelayCommand(DownloadUpdateAsync);
        }

        public void SetAvailableUpdate(UpdateInfo updateInfo)
        {
            AvailableUpdate = updateInfo;
            IsUpdateAvailable = true;
            StatusMessage = $"يتوفر تحديث جديد! الإصدار {updateInfo.Version}";
        }

        private async Task CheckForUpdatesAsync()
        {
            IsChecking = true;
            StatusMessage = "جاري البحث عن تحديثات جديدة...";
            AvailableUpdate = null;
            IsUpdateAvailable = false;

            var update = await _updateService.CheckForUpdatesAsync();

            if (update != null && update.Version != CurrentVersion)
            {
                AvailableUpdate = update;
                IsUpdateAvailable = true;
                StatusMessage = $"يتوفر تحديث جديد! الإصدار {update.Version}";
            }
            else
            {
                IsUpdateAvailable = false;
                StatusMessage = "أنت تستخدم أحدث إصدار من النظام.";
            }

            IsChecking = false;
        }

        private async Task DownloadUpdateAsync()
        {
            if (AvailableUpdate == null) return;

            IsDownloading = true;
            DownloadProgress = 0;
            ProgressText = "0%";
            StatusMessage = "جاري تحميل التحديث داخل البرنامج... يرجى الانتظار.";

            var progress = new Progress<double>(p =>
            {
                DownloadProgress = p;
                ProgressText = $"{(int)p}%";
                StatusMessage = $"جاري تحميل التحديث داخل البرنامج ({ProgressText})...";
            });

            var success = await _updateService.DownloadUpdateAsync(AvailableUpdate.DownloadUrl, progress);

            if (success)
            {
                StatusMessage = "تم تحميل التحديث بنجاح! جاري التثبيت التلقائي الآن...";
            }
            else
            {
                StatusMessage = "فشل التحميل: لم يتم العثور على ملف مثبت (.exe) مرفق بالإصدار على GitHub. يرجى رفع ملف التثبيت (.exe) في مربع المرفقات عند إطلاق Release.";
            }

            IsDownloading = false;
        }
    }
}
