using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Services;
using RetailApp.ViewModels;
using RetailApp.Views;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace RetailApp
{
    public partial class App : Application
    {
        private IHost _host;
        
        public static IServiceProvider ServiceProvider => ((App)Current)._host.Services;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Database
                    services.AddDbContext<AppDbContext>();

                    // Core Services
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IDialogService, DialogService>();
                    services.AddSingleton<INotificationService, NotificationService>();
                    services.AddSingleton<ILoggingService, LoggingService>();
                    services.AddSingleton<IThemeService, ThemeService>();
                    services.AddSingleton<IBackupService, BackupService>();
                    services.AddSingleton<IBackupHistoryService, BackupHistoryService>();
                    services.AddSingleton<IBackupSchedulerService, BackupSchedulerService>();
                    services.AddSingleton<ICompressionService, CompressionService>();
                    services.AddSingleton<IIntegrityVerificationService, IntegrityVerificationService>();
                    services.AddSingleton<IRestoreService, RestoreService>();
                    services.AddHostedService<BackupBackgroundWorker>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<ILicenseService, LicenseService>();
                    services.AddSingleton<IUpdateService, UpdateService>();
                    services.AddSingleton<IMachineIdService, MachineIdService>();
                    services.AddSingleton<ILicenseValidationService, LicenseValidationService>();
                    services.AddSingleton<IActivationService, ActivationService>();
                    services.AddSingleton<IVersionService, VersionService>();
                    
                    // Setup & First Run Services
                    services.AddSingleton<IDirectoryService, DirectoryService>();
                    services.AddSingleton<IEnvironmentCheckService, EnvironmentCheckService>();
                    services.AddSingleton<IMigrationService, MigrationService>();
                    services.AddSingleton<IFirstRunService, FirstRunService>();
                    
                    services.AddTransient<IStatisticsService, MockStatisticsService>();
                    services.AddTransient<IProductService, ProductService>();
                    services.AddTransient<IInvoiceService, MockInvoiceService>();
                    services.AddTransient<IInventoryService, MockInventoryService>();
                    services.AddTransient<ICategoryService, MockCategoryService>();
                    services.AddTransient<IBrandService, MockBrandService>();
                    services.AddTransient<IBarcodeService, MockBarcodeService>();
                    services.AddTransient<IStockMovementService, MockStockMovementService>();
                    services.AddTransient<IPricingService, MockPricingService>();

                    // Sales Services
                    services.AddTransient<ISalesService, SalesService>();
                    services.AddTransient<ISalesManagementService, SalesManagementService>();
                    services.AddTransient<ISalesStatisticsService, SalesStatisticsService>();

                    // Return Services
                    services.AddTransient<ISalesReturnService, SalesReturnService>();
                    services.AddTransient<IPurchaseReturnService, PurchaseReturnService>();
                    services.AddTransient<IReturnStatisticsService, ReturnStatisticsService>();

                    // Installments Services
                    services.AddTransient<IInstallmentService, InstallmentService>();
                    services.AddTransient<ICollectionService, CollectionService>();
                    services.AddTransient<IInstallmentStatisticsService, InstallmentStatisticsService>();

                    // Accounting Services
                    services.AddTransient<IAccountingService, AccountingService>();
                    services.AddTransient<IJournalService, JournalService>();
                    services.AddTransient<IFinancialStatementService, FinancialStatementService>();

                    // Expense & Income Services
                    services.AddTransient<IExpenseService, ExpenseService>();
                    services.AddTransient<IIncomeService, IncomeService>();
                    services.AddTransient<IExpenseStatisticsService, ExpenseStatisticsService>();

                    // HR & Payroll Services
                    services.AddTransient<IEmployeeService, EmployeeService>();
                    services.AddTransient<ILoanAdvanceService, LoanAdvanceService>();
                    services.AddTransient<IPayrollService, PayrollService>();

                    // Reporting & BI Services
                    services.AddTransient<IReportingService, ReportingService>();

                    // Security & Audit Services
                    services.AddSingleton<IAuthenticationService, AuthenticationService>();
                    services.AddScoped<IAuditLogService, AuditLogService>();

                    // Printing Services
                    services.AddSingleton<IPrintTemplateManager, PrintTemplateManager>();
                    services.AddTransient<IPrintService, PrintService>();

                    // Customer Services
                    services.AddTransient<ICustomerService, CustomerService>();
                    services.AddTransient<ICustomerSearchService, CustomerSearchService>();
                    services.AddTransient<ICustomerValidationService, CustomerValidationService>();
                    services.AddTransient<ICustomerStatisticsService, CustomerStatisticsService>();

                    // Supplier Services
                    services.AddTransient<ISupplierService, SupplierService>();
                    services.AddTransient<ISupplierSearchService, SupplierSearchService>();
                    services.AddTransient<ISupplierValidationService, SupplierValidationService>();
                    services.AddTransient<ISupplierStatisticsService, SupplierStatisticsService>();

                    // Purchase Services
                    services.AddTransient<IPurchaseService, PurchaseService>();
                    services.AddTransient<IPurchaseSearchService, PurchaseSearchService>();
                    services.AddTransient<IPurchaseValidationService, PurchaseValidationService>();
                    services.AddTransient<IPurchaseStatisticsService, PurchaseStatisticsService>();
                    services.AddTransient<IPurchaseCalculationService, PurchaseCalculationService>();

                    // ViewModels
                    services.AddSingleton<MainViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<PosViewModel>();
                    services.AddTransient<InventoryViewModel>();
                    services.AddTransient<ProductEditorViewModel>();
                    services.AddTransient<PlaceholderViewModel>();
                    services.AddTransient<CustomersViewModel>();
                    services.AddTransient<CustomerEditorViewModel>();
                    services.AddTransient<CustomerProfileViewModel>();
                    services.AddTransient<SuppliersViewModel>();
                    services.AddTransient<SupplierEditorViewModel>();
                    services.AddTransient<SupplierProfileViewModel>();
                    services.AddTransient<PurchasesViewModel>();
                    services.AddTransient<PurchaseEditorViewModel>();
                    services.AddTransient<SalesViewModel>();
                    
                    services.AddTransient<ReturnsViewModel>();
                    services.AddTransient<SalesReturnEditorViewModel>();
                    services.AddTransient<PurchaseReturnEditorViewModel>();

                    // Installment ViewModels
                    services.AddTransient<InstallmentsViewModel>();

                    // Accounting ViewModels
                    services.AddTransient<ChartOfAccountsViewModel>();
                    services.AddTransient<JournalEntriesViewModel>();
                    services.AddTransient<JournalEditorViewModel>();

                    services.AddTransient<ExpenseEditorViewModel>();
                    services.AddTransient<IncomeEditorViewModel>();

                    // HR ViewModels
                    services.AddTransient<EmployeesViewModel>();
                    services.AddTransient<EmployeeEditorViewModel>();
                    services.AddTransient<PayrollViewModel>();

                    // Reporting ViewModel
                    services.AddTransient<ReportsViewModel>();

                    // Windows and Views
                    services.AddTransient<WarehouseViewModel>();
                    services.AddTransient<ExpensesViewModel>();
                    services.AddTransient<BackupViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<AboutViewModel>();
                    services.AddTransient<LicenseViewModel>();
                    services.AddTransient<DeveloperDashboardViewModel>();
                    services.AddTransient<UpdateViewModel>();
                    services.AddTransient<FirstRunWizardViewModel>();
                    services.AddTransient<PrintDesignerViewModel>();

                    // Views
                    services.AddSingleton<MainWindow>();
                    services.AddTransient<MainView>();
                    services.AddTransient<LoginView>();
                    services.AddTransient<FirstRunWizardView>();
                    services.AddTransient<DashboardView>();
                    services.AddTransient<PlaceholderView>();
                    services.AddTransient<CustomersView>();
                    services.AddTransient<SuppliersView>();
                    services.AddTransient<UsersView>();
                    services.AddTransient<PurchasesView>();
                    services.AddTransient<SalesView>();
                    services.AddTransient<ReturnsView>();
                    services.AddTransient<WarehouseView>();
                    services.AddTransient<InstallmentsView>();
                    services.AddTransient<ExpensesView>();
                    services.AddTransient<ReportsView>();
                    services.AddTransient<BackupView>();
                    services.AddTransient<SettingsView>();
                    services.AddTransient<AboutView>();
                    services.AddTransient<LicenseView>();
                    services.AddTransient<DeveloperDashboardView>();
                    services.AddTransient<UpdateView>();
                    services.AddTransient<RetailApp.Views.Settings.PrintDesignerView>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Hook up Global Exception Handling
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            await _host.StartAsync();

            using (var scope = _host.Services.CreateScope())
            {
                // Initialize directories first
                var dirService = scope.ServiceProvider.GetRequiredService<IDirectoryService>();
                dirService.InitializeDirectories();

                // Environment & Directory Checks
                var envCheck = scope.ServiceProvider.GetRequiredService<IEnvironmentCheckService>();
                if (!envCheck.ValidateEnvironment(out string errorMsg))
                {
                    System.Windows.MessageBox.Show(errorMsg, "فشل بدء التشغيل", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    Application.Current.Shutdown();
                    return;
                }
            }

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            // Setup Navigation & First Run Check
            var navigationService = _host.Services.GetRequiredService<INavigationService>();
            
            using (var scope = _host.Services.CreateScope())
            {
                var firstRunService = scope.ServiceProvider.GetRequiredService<IFirstRunService>();
                var isFirstRun = await firstRunService.IsFirstRunRequiredAsync();

                if (isFirstRun)
                {
                    navigationService.NavigateTo<FirstRunWizardViewModel>();
                }
                else
                {
                    // Licensing Check
                    var licenseService = scope.ServiceProvider.GetRequiredService<ILicenseService>();
                    var status = await licenseService.ValidateCurrentLicenseAsync();
                    
                    if (status == Models.LicenseStatus.Active || status == Models.LicenseStatus.Trial)
                    {
                        navigationService.NavigateTo<LoginViewModel>();
                    }
                    else if (status == Models.LicenseStatus.Expired)
                    {
                        MessageBox.Show("انتهت صلاحية اشتراكك في البرنامج!\nيرجى التواصل مع المطور وإدخال كود تفعيل جديد لتجديد الاشتراك.", "تنبيه انتهاء الاشتراك ⚠️", MessageBoxButton.OK, MessageBoxImage.Warning);
                        navigationService.NavigateTo<LicenseViewModel>();
                    }
                    else
                    {
                        navigationService.NavigateTo<LicenseViewModel>();
                    }
                }
            }

            using (var scope = _host.Services.CreateScope())
            {
                var themeService = scope.ServiceProvider.GetRequiredService<IThemeService>();
                await themeService.ApplyInitialThemeAsync();

                var schedulerService = scope.ServiceProvider.GetRequiredService<IBackupSchedulerService>();
                var config = await schedulerService.GetScheduleConfigAsync();
                if (config.Frequency == Models.BackupFrequency.OnStartup)
                {
                    var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                    await backupService.CreateBackupAsync(Models.BackupType.OnStartup);
                    await schedulerService.EnforceRetentionPolicyAsync();
                }
            }

            base.OnStartup(e);
        }

        private bool _hasShownException = false;
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!_hasShownException)
            {
                _hasShownException = true;
                System.Windows.MessageBox.Show($"حدث خطأ غير متوقع في واجهة المستخدم:\n{e.Exception.Message}\nLine: {(e.Exception as System.Windows.Markup.XamlParseException)?.LineNumber}", "خطأ في النظام", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            e.Handled = true; // Prevent app crash
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                System.Windows.MessageBox.Show($"حدث خطأ غير متوقع في النظام:\n{ex.Message}", "خطأ فادح", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (var scope = _host.Services.CreateScope())
            {
                var schedulerService = scope.ServiceProvider.GetRequiredService<IBackupSchedulerService>();
                var config = await schedulerService.GetScheduleConfigAsync();
                if (config.Frequency == Models.BackupFrequency.OnExit)
                {
                    var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                    await backupService.CreateBackupAsync(Models.BackupType.OnExit);
                    await schedulerService.EnforceRetentionPolicyAsync();
                }
            }

            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
