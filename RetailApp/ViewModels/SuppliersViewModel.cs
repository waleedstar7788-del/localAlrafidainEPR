using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class SuppliersViewModel : BaseViewModel
    {
        private readonly ISupplierService _supplierService;
        private readonly ISupplierStatisticsService _statisticsService;
        private readonly ISupplierSearchService _searchService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers = new();

        [ObservableProperty]
        private int _totalSuppliers;

        [ObservableProperty]
        private int _newSuppliersThisMonth;

        [ObservableProperty]
        private int _activeSuppliers;

        [ObservableProperty]
        private int _debtors;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        private const int PageSize = 50;

        public SuppliersViewModel(
            ISupplierService supplierService,
            ISupplierStatisticsService statisticsService,
            ISupplierSearchService searchService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _supplierService = supplierService;
            _statisticsService = statisticsService;
            _searchService = searchService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            await LoadStatisticsAsync();
            await LoadSuppliersAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            TotalSuppliers = await _statisticsService.GetTotalSuppliersAsync();
            NewSuppliersThisMonth = await _statisticsService.GetNewSuppliersThisMonthAsync();
            ActiveSuppliers = await _statisticsService.GetActiveSuppliersAsync();
            Debtors = await _statisticsService.GetSuppliersWithOutstandingBalanceAsync();
        }

        private async Task LoadSuppliersAsync()
        {
            Suppliers.Clear();
            var list = await _supplierService.GetSuppliersAsync(CurrentPage, PageSize);
            foreach (var s in list) Suppliers.Add(s);
        }

        partial void OnSearchQueryChanged(string value)
        {
            PerformSearchAsync(value).ConfigureAwait(false);
        }

        private async Task PerformSearchAsync(string query)
        {
            CurrentPage = 1;
            Suppliers.Clear();
            var list = await _searchService.SearchSuppliersAsync(query, CurrentPage, PageSize);
            foreach (var s in list) Suppliers.Add(s);
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            CurrentPage++;
            if (string.IsNullOrWhiteSpace(SearchQuery))
                await LoadSuppliersAsync();
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
                    await LoadSuppliersAsync();
                else
                    await PerformSearchAsync(SearchQuery);
            }
        }

        [RelayCommand]
        private async Task AddSupplierAsync()
        {
            var editorViewModel = new SupplierEditorViewModel(_supplierService, null);
            bool result = await _dialogService.ShowDialogAsync("SupplierEditorDialog", editorViewModel);
            if (result)
            {
                _notificationService.ShowSuccess("تم إضافة المورد بنجاح.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task EditSupplierAsync(Supplier supplier)
        {
            if (supplier == null) return;
            var editorViewModel = new SupplierEditorViewModel(_supplierService, supplier);
            bool result = await _dialogService.ShowDialogAsync("SupplierEditorDialog", editorViewModel);
            if (result)
            {
                _notificationService.ShowSuccess("تم تحديث بيانات المورد بنجاح.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteSupplierAsync(Supplier supplier)
        {
            if (supplier == null) return;
            bool confirm = await _dialogService.ShowConfirmationAsync($"هل أنت متأكد من حذف المورد {supplier.SupplierName}؟");
            if (confirm)
            {
                await _supplierService.DeleteSupplierAsync(supplier.Id);
                _notificationService.ShowSuccess("تم حذف المورد بنجاح.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task OpenProfileAsync(Supplier supplier)
        {
            if (supplier == null) return;
            var profileViewModel = new SupplierProfileViewModel(_supplierService);
            profileViewModel.Initialize(supplier);
            await _dialogService.ShowDialogAsync("SupplierProfileDialog", profileViewModel);
        }
    }
}
