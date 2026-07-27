using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class ReturnsViewModel : BaseViewModel
    {
        private readonly ISalesReturnService _salesReturnService;
        private readonly IPurchaseReturnService _purchaseReturnService;
        private readonly IReturnStatisticsService _statisticsService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        // Collections
        [ObservableProperty] private ObservableCollection<SalesReturnInvoice> _salesReturns = new();
        [ObservableProperty] private ObservableCollection<PurchaseReturnInvoice> _purchaseReturns = new();

        // Stats
        [ObservableProperty] private decimal _monthlySalesReturnTotal;
        [ObservableProperty] private decimal _monthlyPurchaseReturnTotal;
        [ObservableProperty] private int _todayReturnsCount;

        [ObservableProperty] private int _currentSalesPage = 1;
        [ObservableProperty] private int _currentPurchasePage = 1;
        private const int PageSize = 50;

        public ReturnsViewModel(
            ISalesReturnService salesReturnService,
            IPurchaseReturnService purchaseReturnService,
            IReturnStatisticsService statisticsService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _salesReturnService = salesReturnService;
            _purchaseReturnService = purchaseReturnService;
            _statisticsService = statisticsService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            LoadDataAsync().ConfigureAwait(false);
        }

        public async Task LoadDataAsync()
        {
            await LoadStatisticsAsync();
            await LoadSalesReturnsAsync();
            await LoadPurchaseReturnsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            MonthlySalesReturnTotal = await _statisticsService.GetMonthlySalesReturnTotalAsync();
            MonthlyPurchaseReturnTotal = await _statisticsService.GetMonthlyPurchaseReturnTotalAsync();
            TodayReturnsCount = await _statisticsService.GetTodayReturnsCountAsync();
        }

        private async Task LoadSalesReturnsAsync()
        {
            SalesReturns.Clear();
            var list = await _salesReturnService.GetReturnsAsync(CurrentSalesPage, PageSize);
            foreach (var r in list) SalesReturns.Add(r);
        }

        private async Task LoadPurchaseReturnsAsync()
        {
            PurchaseReturns.Clear();
            var list = await _purchaseReturnService.GetReturnsAsync(CurrentPurchasePage, PageSize);
            foreach (var r in list) PurchaseReturns.Add(r);
        }

        // --- Commands ---

        [RelayCommand]
        private async Task OpenSalesReturnEditorAsync()
        {
            var editorViewModel = App.ServiceProvider.GetService(typeof(SalesReturnEditorViewModel)) as SalesReturnEditorViewModel;
            bool result = await _dialogService.ShowDialogAsync("SalesReturnDialog", editorViewModel);
            if (result)
            {
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task OpenPurchaseReturnEditorAsync()
        {
            var editorViewModel = App.ServiceProvider.GetService(typeof(PurchaseReturnEditorViewModel)) as PurchaseReturnEditorViewModel;
            bool result = await _dialogService.ShowDialogAsync("PurchaseReturnDialog", editorViewModel);
            if (result)
            {
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task CancelSalesReturnAsync(SalesReturnInvoice invoice)
        {
            if (invoice != null && invoice.Status != ReturnStatus.Cancelled)
            {
                await _salesReturnService.CancelReturnAsync(invoice.Id);
                _notificationService.ShowSuccess("تم إلغاء مرتجع المبيعات واسترجاع الحسابات.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task CancelPurchaseReturnAsync(PurchaseReturnInvoice invoice)
        {
            if (invoice != null && invoice.Status != ReturnStatus.Cancelled)
            {
                await _purchaseReturnService.CancelReturnAsync(invoice.Id);
                _notificationService.ShowSuccess("تم إلغاء مرتجع المشتريات واسترجاع الحسابات.");
                await LoadDataAsync();
            }
        }
    }
}
