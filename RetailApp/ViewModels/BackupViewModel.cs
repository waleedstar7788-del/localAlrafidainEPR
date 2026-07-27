using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.ViewModels
{
    public partial class BackupViewModel : BaseViewModel
    {
        private readonly IBackupService _backupService;
        private readonly IRestoreService _restoreService;
        private readonly IBackupHistoryService _historyService;
        private readonly IBackupSchedulerService _schedulerService;
        private readonly IDialogService _dialogService;
        private CancellationTokenSource _cancellationTokenSource;

        [ObservableProperty]
        private ObservableCollection<BackupHistoryItem> _backupHistory;

        [ObservableProperty]
        private BackupScheduleConfig _scheduleConfig;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _isWorking;

        [ObservableProperty]
        private string _progressText;

        public ICommand CreateBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand CancelCommand { get; }

        public BackupViewModel(
            IBackupService backupService,
            IRestoreService restoreService,
            IBackupHistoryService historyService,
            IBackupSchedulerService schedulerService,
            IDialogService dialogService)
        {
            _backupService = backupService;
            _restoreService = restoreService;
            _historyService = historyService;
            _schedulerService = schedulerService;
            _dialogService = dialogService;

            _statusMessage = string.Empty;
            _progressText = string.Empty;
            _cancellationTokenSource = new CancellationTokenSource();

            BackupHistory = new ObservableCollection<BackupHistoryItem>();
            ScheduleConfig = new BackupScheduleConfig();
            
            CreateBackupCommand = new AsyncRelayCommand(CreateBackupAsync, () => !IsWorking);
            RestoreBackupCommand = new AsyncRelayCommand<BackupHistoryItem>(RestoreBackupAsync, _ => !IsWorking);
            SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync, () => !IsWorking);
            CancelCommand = new RelayCommand(CancelOperation, () => IsWorking);

            _backupService.ProgressChanged += OnProgressChanged;
            _restoreService.ProgressChanged += OnProgressChanged;

            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var history = await _historyService.GetHistoryAsync();
                BackupHistory = new ObservableCollection<BackupHistoryItem>(history.OrderByDescending(h => h.Date));

                ScheduleConfig = await _schedulerService.GetScheduleConfigAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"خطأ في تحميل البيانات: {ex.Message}";
            }
        }

        private void OnProgressChanged(object? sender, string message)
        {
            ProgressText = message;
        }

        private async Task CreateBackupAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            IsWorking = true;
            StatusMessage = string.Empty;
            ProgressText = "بدء النسخ الاحتياطي...";
            
            try
            {
                var result = await _backupService.CreateBackupAsync(BackupType.Manual, null, _cancellationTokenSource.Token);
                
                if (result.Status == BackupStatus.Success)
                {
                    StatusMessage = "تم إنشاء النسخة الاحتياطية بنجاح.";
                }
                else
                {
                    StatusMessage = $"حدث خطأ: {result.ErrorMessage}";
                }

                LoadDataAsync();
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "تم إلغاء العملية.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"حدث خطأ غير متوقع: {ex.Message}";
            }
            finally
            {
                IsWorking = false;
                ProgressText = string.Empty;
            }
        }

        private async Task RestoreBackupAsync(BackupHistoryItem? item)
        {
            if (item == null) return;

            bool confirm = await _dialogService.ShowConfirmationAsync("تحذير: سيتم استبدال جميع البيانات الحالية بالبيانات الموجودة في هذه النسخة. هل أنت متأكد من الاستمرار؟");
            
            if (!confirm) return;

            _cancellationTokenSource = new CancellationTokenSource();
            IsWorking = true;
            StatusMessage = string.Empty;
            ProgressText = "بدء الاستعادة...";

            try
            {
                bool success = await _restoreService.RestoreBackupAsync(item.Location, _cancellationTokenSource.Token);
                
                if (success)
                {
                    StatusMessage = "تم استعادة النسخة الاحتياطية بنجاح. سيتم إعادة تشغيل التطبيق.";
                    await Task.Delay(2000);
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    StatusMessage = "فشلت عملية الاستعادة. تحقق من السجلات للحصول على تفاصيل.";
                }
            }
            catch (OperationCanceledException)
            {
                StatusMessage = "تم إلغاء الاستعادة.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"خطأ غير متوقع أثناء الاستعادة: {ex.Message}";
            }
            finally
            {
                IsWorking = false;
                ProgressText = string.Empty;
            }
        }

        private async Task SaveSettingsAsync()
        {
            try
            {
                IsWorking = true;
                await _schedulerService.SaveScheduleConfigAsync(ScheduleConfig);
                StatusMessage = "تم حفظ إعدادات النسخ الاحتياطي التلقائي بنجاح.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"خطأ في حفظ الإعدادات: {ex.Message}";
            }
            finally
            {
                IsWorking = false;
            }
        }

        private void CancelOperation()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
