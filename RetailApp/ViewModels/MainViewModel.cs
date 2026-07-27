using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RetailApp.Interfaces;
using System;
using System.Collections.Generic;

namespace RetailApp.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILicenseService _licenseService;
        
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private ObservableObject? _innerCurrentView;

        [ObservableProperty]
        private string _breadcrumb = "الرئيسية";

        [ObservableProperty]
        private string _licenseRemainingText = "جاري التحقق...";

        [ObservableProperty]
        private bool _isLicenseActivated;

        private readonly Stack<ObservableObject> _backHistory = new();
        private readonly Stack<ObservableObject> _forwardHistory = new();

        [ObservableProperty]
        private bool _hasAvailableUpdate;

        [ObservableProperty]
        private string _updateNotificationText = string.Empty;

        [ObservableProperty]
        private Models.UpdateInfo? _availableUpdateInfo;

        public MainViewModel(IServiceProvider serviceProvider, INavigationService navigationService, ILicenseService licenseService)
        {
            _serviceProvider = serviceProvider;
            _navigation = navigationService;
            _licenseService = licenseService;
            
            NavigateToDashboard();
            LoadLicenseStatusAsync();
            CheckForUpdateInBackgroundAsync();

            if (_serviceProvider.GetService(typeof(IActivationService)) is IActivationService activationService)
            {
                activationService.OnLicenseChanged += () =>
                {
                    System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                    {
                        LoadLicenseStatusAsync();
                    });
                };
            }
        }

        private async void CheckForUpdateInBackgroundAsync()
        {
            try
            {
                if (_serviceProvider.GetService(typeof(IUpdateService)) is IUpdateService updateService)
                {
                    var update = await updateService.CheckForUpdatesAsync();
                    var versionService = _serviceProvider.GetService(typeof(IVersionService)) as IVersionService;
                    string currentVersion = versionService?.GetCurrentVersion() ?? "1.0.0";

                    if (update != null && update.Version != currentVersion)
                    {
                        AvailableUpdateInfo = update;
                        UpdateNotificationText = $"يتوفر تحديث جديد للبرنامج الإصدار ({update.Version})! اضغط هنا للاطلاع على الإضافات والتحديث المباشر.";
                        HasAvailableUpdate = true;
                    }
                }
            }
            catch
            {
                // التجاهل في حالة انقطاع الإنترنت أو الخطأ لعدم إزعاج المستخدم
            }
        }

        [RelayCommand]
        private void DismissUpdateNotification()
        {
            HasAvailableUpdate = false;
        }

        public async void LoadLicenseStatusAsync()
        {
            try
            {
                var license = await _licenseService.GetCurrentLicenseAsync();
                var status = await _licenseService.ValidateCurrentLicenseAsync();
                if (status == Models.LicenseStatus.Active || status == Models.LicenseStatus.Trial)
                {
                    IsLicenseActivated = true;
                    if (license?.SubscriptionType == Models.LicenseType.Lifetime)
                    {
                        LicenseRemainingText = "الاشتراك: مدى الحياة ♾️";
                    }
                    else if (license != null)
                    {
                        LicenseRemainingText = $"متبقي: {license.RemainingDays} يومًا ⏳";
                    }
                    else
                    {
                        LicenseRemainingText = "الاشتراك مفعل ✔";
                    }
                }
                else
                {
                    IsLicenseActivated = false;
                    LicenseRemainingText = "الاشتراك غير مفعل ⚠️";
                }
            }
            catch
            {
                IsLicenseActivated = false;
                LicenseRemainingText = "الاشتراك غير مفعل ⚠️";
            }
        }

        [RelayCommand]
        private void Logout()
        {
            Navigation.NavigateTo<LoginViewModel>();
        }

        [RelayCommand]
        private void GoBack()
        {
            if (_backHistory.Count > 0 && InnerCurrentView != null)
            {
                _forwardHistory.Push(InnerCurrentView);
                InnerCurrentView = _backHistory.Pop();
                UpdateBreadcrumb(InnerCurrentView);
            }
        }

        [RelayCommand]
        private void GoForward()
        {
            if (_forwardHistory.Count > 0 && InnerCurrentView != null)
            {
                _backHistory.Push(InnerCurrentView);
                InnerCurrentView = _forwardHistory.Pop();
                UpdateBreadcrumb(InnerCurrentView);
            }
        }

        public void NavigateInner(ObservableObject viewModel)
        {
            if (InnerCurrentView != null)
            {
                _backHistory.Push(InnerCurrentView);
                _forwardHistory.Clear();
            }
            InnerCurrentView = viewModel;
            UpdateBreadcrumb(viewModel);
        }

        private void UpdateBreadcrumb(ObservableObject vm)
        {
            Breadcrumb = vm.GetType().Name switch
            {
                nameof(DashboardViewModel) => "الرئيسية",
                nameof(PosViewModel) => "نقطة البيع (POS)",
                nameof(SalesViewModel) => "فواتير المبيعات",
                nameof(PurchasesViewModel) => "فواتير المشتروات",
                nameof(ReturnsViewModel) => "المرتجعات",
                nameof(InstallmentsViewModel) => "الأقساط والديون",
                nameof(InventoryViewModel) => "المنتجات والمخزون",
                nameof(WarehouseViewModel) => "المستودعات",
                nameof(CustomersViewModel) => "إدارة العملاء",
                nameof(SuppliersViewModel) => "إدارة الموردين",
                nameof(ExpensesViewModel) => "المصاريف والإيرادات",
                nameof(ChartOfAccountsViewModel) => "شجرة الحسابات",
                nameof(JournalEntriesViewModel) => "دفتر اليومية",
                nameof(ReportsViewModel) => "التقارير المالية",
                nameof(EmployeesViewModel) => "الموظفون",
                nameof(PayrollViewModel) => "مسير الرواتب",
                nameof(UsersViewModel) => "إدارة المستخدمين",
                nameof(BackupViewModel) => "النسخ الاحتياطي",
                nameof(SettingsViewModel) => "إعدادات النظام",
                nameof(AboutViewModel) => "حول البرنامج",
                nameof(DeveloperDashboardViewModel) => "لوحة المطور",
                nameof(UpdateViewModel) => "تحديثات النظام",
                nameof(LicenseViewModel) => "ترخيص النظام",
                _ => "الرئيسية"
            };
        }

        private T GetVm<T>() where T : class
        {
            return _serviceProvider.GetService(typeof(T)) as T ?? _serviceProvider.GetRequiredService<T>();
        }

        [RelayCommand] private void NavigateToDashboard() => NavigateInner(GetVm<DashboardViewModel>());
        [RelayCommand] private void NavigateToPos() => NavigateInner(GetVm<PosViewModel>());
        [RelayCommand] private void NavigateToSales() => NavigateInner(GetVm<SalesViewModel>());
        [RelayCommand] private void NavigateToPurchases() => NavigateInner(GetVm<PurchasesViewModel>());
        [RelayCommand] private void NavigateToReturns() => NavigateInner(GetVm<ReturnsViewModel>());
        [RelayCommand] private void NavigateToInstallments() => NavigateInner(GetVm<InstallmentsViewModel>());
        [RelayCommand] private void NavigateToInventory() => NavigateInner(GetVm<InventoryViewModel>());
        [RelayCommand] private void NavigateToWarehouse() => NavigateInner(GetVm<WarehouseViewModel>());
        [RelayCommand] private void NavigateToCustomers() => NavigateInner(GetVm<CustomersViewModel>());
        [RelayCommand] private void NavigateToSuppliers() => NavigateInner(GetVm<SuppliersViewModel>());
        [RelayCommand] private void NavigateToExpenses() => NavigateInner(GetVm<ExpensesViewModel>());
        [RelayCommand] private void NavigateToChartOfAccounts() => NavigateInner(GetVm<ChartOfAccountsViewModel>());
        [RelayCommand] private void NavigateToJournalEntries() => NavigateInner(GetVm<JournalEntriesViewModel>());
        [RelayCommand] private void NavigateToReports() => NavigateInner(GetVm<ReportsViewModel>());
        [RelayCommand] private void NavigateToEmployees() => NavigateInner(GetVm<EmployeesViewModel>());
        [RelayCommand] private void NavigateToPayroll() => NavigateInner(GetVm<PayrollViewModel>());
        [RelayCommand] private void NavigateToUsers() => NavigateInner(GetVm<UsersViewModel>());
        [RelayCommand] private void NavigateToBackup() => NavigateInner(GetVm<BackupViewModel>());
        [RelayCommand] private void NavigateToSettings() => NavigateInner(GetVm<SettingsViewModel>());
        [RelayCommand] private void NavigateToAbout() => NavigateInner(GetVm<AboutViewModel>());
        [RelayCommand] private void NavigateToDeveloperDashboard() => NavigateInner(GetVm<DeveloperDashboardViewModel>());
        [RelayCommand]
        private void NavigateToUpdateView()
        {
            var updateVm = GetVm<UpdateViewModel>();
            if (AvailableUpdateInfo != null)
            {
                updateVm.SetAvailableUpdate(AvailableUpdateInfo);
            }
            NavigateInner(updateVm);
        }
        [RelayCommand] private void NavigateToLicense() => NavigateInner(GetVm<LicenseViewModel>());
    }
}
