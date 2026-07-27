using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class SalesViewModel : BaseViewModel
    {
        private readonly ISalesManagementService _salesService;
        private readonly ISalesStatisticsService _statisticsService;
        private readonly INotificationService _notificationService;
        private readonly IDialogService _dialogService;
        private readonly IProductService _productService;
        private readonly IPrintService _printService;
        private readonly IPrintTemplateManager _printTemplateManager;

        [ObservableProperty]
        private ObservableCollection<SalesInvoice> _invoices = new();

        [ObservableProperty]
        private SalesInvoice? _selectedInvoice;

        [ObservableProperty]
        private decimal _todaySalesTotal;

        [ObservableProperty]
        private decimal _monthlySalesTotal;

        [ObservableProperty]
        private decimal _todayProfit;

        [ObservableProperty]
        private decimal _monthlyProfit;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        private const int PageSize = 50;

        public SalesViewModel(
            ISalesManagementService salesService,
            ISalesStatisticsService statisticsService,
            INotificationService notificationService,
            IDialogService dialogService,
            IProductService productService,
            IPrintService printService,
            IPrintTemplateManager printTemplateManager)
        {
            _salesService = salesService;
            _statisticsService = statisticsService;
            _notificationService = notificationService;
            _dialogService = dialogService;
            _productService = productService;
            _printService = printService;
            _printTemplateManager = printTemplateManager;

            LoadDataAsync().ConfigureAwait(false);
        }

        public async Task LoadDataAsync()
        {
            await LoadStatisticsAsync();
            await LoadInvoicesAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            TodaySalesTotal = await _statisticsService.GetTodaySalesTotalAsync();
            MonthlySalesTotal = await _statisticsService.GetMonthlySalesTotalAsync();
            TodayProfit = await _statisticsService.GetTodayProfitAsync();
            MonthlyProfit = await _statisticsService.GetMonthlyProfitAsync();
        }

        private async Task LoadInvoicesAsync()
        {
            Invoices.Clear();
            var list = await _salesService.GetInvoicesAsync(CurrentPage, PageSize);
            foreach (var inv in list) Invoices.Add(inv);
        }

        partial void OnSearchQueryChanged(string value)
        {
            PerformSearchAsync(value).ConfigureAwait(false);
        }

        private async Task PerformSearchAsync(string query)
        {
            CurrentPage = 1;
            Invoices.Clear();
            var list = await _salesService.SearchInvoicesAsync(query, CurrentPage, PageSize);
            foreach (var inv in list) Invoices.Add(inv);
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            CurrentPage++;
            if (string.IsNullOrWhiteSpace(SearchQuery))
                await LoadInvoicesAsync();
            else
                await PerformSearchAsync(SearchQuery);
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                if (string.IsNullOrWhiteSpace(SearchQuery))
                    await LoadInvoicesAsync();
                else
                    await PerformSearchAsync(SearchQuery);
            }
        }

        [RelayCommand]
        private async Task EditInvoiceAsync(SalesInvoice invoice)
        {
            if (invoice == null) return;

            if (invoice.Status == SalesInvoiceStatus.Cancelled)
            {
                _notificationService.ShowWarning("لا يمكن تعديل فاتورة ملغية.");
                return;
            }

            // Load full invoice with items
            var fullInvoice = await _salesService.GetInvoiceWithItemsAsync(invoice.Id);
            if (fullInvoice == null)
            {
                _notificationService.ShowError("تعذر تحميل الفاتورة.");
                return;
            }

            var editorVm = new SalesInvoiceEditorViewModel(_salesService, _productService, _notificationService, fullInvoice);
            var result = await _dialogService.ShowDialogAsync("SalesInvoiceEditorDialog", editorVm);
            
            if (result)
            {
                await LoadDataAsync(); // Refresh everything
            }
        }

        [RelayCommand]
        private async Task ViewInvoiceDetailsAsync(SalesInvoice invoice)
        {
            if (invoice == null) return;

            // Load full invoice with items
            var fullInvoice = await _salesService.GetInvoiceWithItemsAsync(invoice.Id);
            if (fullInvoice == null)
            {
                _notificationService.ShowError("تعذر تحميل الفاتورة.");
                return;
            }

            SelectedInvoice = fullInvoice;
        }

        [RelayCommand]
        private void CloseDetails()
        {
            SelectedInvoice = null;
        }

        [RelayCommand]
        private async Task CancelInvoiceAsync(SalesInvoice invoice)
        {
            if (invoice != null && invoice.Status != SalesInvoiceStatus.Cancelled)
            {
                var confirmed = await _dialogService.ShowConfirmationAsync("هل أنت متأكد من إلغاء هذه الفاتورة؟ سيتم إعادة المخزون.");
                if (confirmed)
                {
                    await _salesService.CancelInvoiceAsync(invoice.Id);
                    _notificationService.ShowSuccess("تم إلغاء الفاتورة واسترجاع المخزون بنجاح.");
                    await LoadDataAsync();
                }
            }
        }

        [RelayCommand]
        private async Task PrintInvoiceAsync(SalesInvoice invoice)
        {
            if (invoice != null)
            {
                var fullInvoice = await _salesService.GetInvoiceWithItemsAsync(invoice.Id);
                if (fullInvoice == null) return;

                _notificationService.ShowInfo($"جاري تجهيز الفاتورة رقم {invoice.InvoiceNumber} للطباعة...");
                
                // Get the default template for SalesInvoice
                var template = await _printTemplateManager.GetDefaultTemplateAsync("SalesInvoice", "A4");
                
                // You can also read LocalSettings.A4PrinterName and use it here
                await _printService.PrintDocumentAsync(fullInvoice, template, null, true);
            }
        }
    }
}
