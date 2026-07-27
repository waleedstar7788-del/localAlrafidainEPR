using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class PurchasesViewModel : BaseViewModel
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IPurchaseStatisticsService _statisticsService;
        private readonly IPurchaseSearchService _searchService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<PurchaseInvoice> _invoices = new();

        [ObservableProperty]
        private int _todayPurchasesCount;

        [ObservableProperty]
        private decimal _monthlyPurchasesValue;

        [ObservableProperty]
        private decimal _outstandingPurchasesValue;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        private const int PageSize = 50;

        public PurchasesViewModel(
            IPurchaseService purchaseService,
            IPurchaseStatisticsService statisticsService,
            IPurchaseSearchService searchService,
            IDialogService dialogService)
        {
            _purchaseService = purchaseService;
            _statisticsService = statisticsService;
            _searchService = searchService;
            _dialogService = dialogService;

            LoadDataAsync().ConfigureAwait(false);
        }

        public async Task LoadDataAsync()
        {
            await LoadStatisticsAsync();
            await LoadInvoicesAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            TodayPurchasesCount = await _statisticsService.GetTodayPurchasesCountAsync();
            MonthlyPurchasesValue = await _statisticsService.GetMonthlyPurchasesValueAsync();
            OutstandingPurchasesValue = await _statisticsService.GetOutstandingPurchasesValueAsync();
        }

        private async Task LoadInvoicesAsync()
        {
            Invoices.Clear();
            var list = await _purchaseService.GetInvoicesAsync(CurrentPage, PageSize);
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
            var list = await _searchService.SearchInvoicesAsync(query, CurrentPage, PageSize);
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
        private async Task AddPurchaseAsync()
        {
            var editorViewModel = App.ServiceProvider.GetService(typeof(PurchaseEditorViewModel)) as PurchaseEditorViewModel;
            bool result = await _dialogService.ShowDialogAsync("PurchaseEditorDialog", editorViewModel);
            if (result)
            {
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task ViewDetailsAsync(PurchaseInvoice invoice)
        {
            if (invoice != null)
            {
                // Similar to Add, but we pass the existing invoice to view/edit
                var editorViewModel = App.ServiceProvider.GetService(typeof(PurchaseEditorViewModel)) as PurchaseEditorViewModel;
                if (editorViewModel != null)
                {
                    // Assuming there is a load method
                    // await editorViewModel.LoadInvoiceAsync(invoice.Id);
                    await _dialogService.ShowDialogAsync("PurchaseEditorDialog", editorViewModel);
                    await LoadDataAsync();
                }
            }
        }
    }
}
