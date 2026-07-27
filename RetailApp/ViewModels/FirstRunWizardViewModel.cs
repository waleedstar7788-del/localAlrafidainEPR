using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;

namespace RetailApp.ViewModels
{
    public partial class FirstRunWizardViewModel : BaseViewModel
    {
        private readonly IMigrationService _migrationService;
        private readonly IDirectoryService _directoryService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _companyName = string.Empty;

        [ObservableProperty]
        private string _adminUsername = "admin";

        [ObservableProperty]
        private string _adminPassword = string.Empty;

        [ObservableProperty]
        private string _adminPasswordConfirm = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isProcessing;

        public ICommand CompleteSetupCommand { get; }

        public FirstRunWizardViewModel(
            IMigrationService migrationService,
            IDirectoryService directoryService,
            INavigationService navigationService)
        {
            _migrationService = migrationService;
            _directoryService = directoryService;
            _navigationService = navigationService;

            CompleteSetupCommand = new AsyncRelayCommand(CompleteSetupAsync);
        }

        private async Task CompleteSetupAsync()
        {
            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                StatusMessage = "يرجى إدخال اسم الشركة.";
                return;
            }

            if (string.IsNullOrWhiteSpace(AdminUsername) || string.IsNullOrWhiteSpace(AdminPassword))
            {
                StatusMessage = "يرجى إدخال بيانات حساب المدير بشكل كامل.";
                return;
            }

            if (AdminPassword != AdminPasswordConfirm)
            {
                StatusMessage = "كلمة المرور غير متطابقة.";
                return;
            }

            IsProcessing = true;
            StatusMessage = "جاري إنشاء قواعد البيانات وتهيئة النظام...";

            try
            {
                // 1. Ensure Directories exist
                _directoryService.InitializeDirectories();

                // 2. Run Database Migrations
                await _migrationService.MigrateDatabaseAsync();

                // 3. Seed Data
                await _migrationService.SeedDefaultDataAsync(AdminUsername, AdminPassword, CompanyName);

                StatusMessage = "تمت تهيئة النظام بنجاح!";
                await Task.Delay(1500);

                // Setup Complete -> Go to Login (or License Validation)
                // We restart the navigation cycle
                _navigationService.NavigateTo<LoginViewModel>();
            }
            catch (Exception ex)
            {
                StatusMessage = $"حدث خطأ أثناء التهيئة: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
