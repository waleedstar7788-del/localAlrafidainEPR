using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class CustomersViewModel : BaseViewModel
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerStatisticsService _statisticsService;
        private readonly ICustomerSearchService _searchService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly ISalesManagementService _salesManagementService;

        [ObservableProperty]
        private ObservableCollection<Customer> _customers = new();

        [ObservableProperty]
        private int _totalCustomers;

        [ObservableProperty]
        private int _newCustomersThisMonth;

        [ObservableProperty]
        private int _vipCustomers;

        [ObservableProperty]
        private int _debtors;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private int _currentPage = 1;

        private const int PageSize = 50;

        public CustomersViewModel(
            ICustomerService customerService,
            ICustomerStatisticsService statisticsService,
            ICustomerSearchService searchService,
            IDialogService dialogService,
            INotificationService notificationService,
            ISalesManagementService salesManagementService)
        {
            _customerService = customerService;
            _statisticsService = statisticsService;
            _searchService = searchService;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _salesManagementService = salesManagementService;

            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            await LoadStatisticsAsync();
            await LoadCustomersAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            TotalCustomers = await _statisticsService.GetTotalCustomersAsync();
            NewCustomersThisMonth = await _statisticsService.GetNewCustomersThisMonthAsync();
            VipCustomers = await _statisticsService.GetVipCustomersCountAsync();
            Debtors = await _statisticsService.GetCustomersWithDebtCountAsync();
        }

        private async Task LoadCustomersAsync()
        {
            Customers.Clear();
            var list = await _customerService.GetCustomersAsync(CurrentPage, PageSize);
            foreach (var c in list) Customers.Add(c);
        }

        partial void OnSearchQueryChanged(string value)
        {
            // Debounce logic should go here ideally, but we'll do direct search for simplicity
            PerformSearchAsync(value).ConfigureAwait(false);
        }

        private async Task PerformSearchAsync(string query)
        {
            CurrentPage = 1;
            Customers.Clear();
            var list = await _searchService.SearchCustomersAsync(query, CurrentPage, PageSize);
            foreach (var c in list) Customers.Add(c);
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            CurrentPage++;
            if (string.IsNullOrWhiteSpace(SearchQuery))
                await LoadCustomersAsync();
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
                    await LoadCustomersAsync();
                else
                    await PerformSearchAsync(SearchQuery);
            }
        }

        [RelayCommand]
        private async Task AddCustomerAsync()
        {
            var editorViewModel = new CustomerEditorViewModel(_customerService, null);
            bool result = await _dialogService.ShowDialogAsync("CustomerEditorDialog", editorViewModel);
            if (result)
            {
                _notificationService.ShowSuccess("تم إضافة العميل بنجاح.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task EditCustomerAsync(Customer customer)
        {
            if (customer == null) return;
            var editorViewModel = new CustomerEditorViewModel(_customerService, customer);
            bool result = await _dialogService.ShowDialogAsync("CustomerEditorDialog", editorViewModel);
            if (result)
            {
                _notificationService.ShowSuccess("تم تحديث بيانات العميل بنجاح.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteCustomerAsync(Customer customer)
        {
            if (customer == null) return;
            bool confirm = await _dialogService.ShowConfirmationAsync($"هل أنت متأكد من حذف العميل {customer.FullName}؟");
            if (confirm)
            {
                await _customerService.DeleteCustomerAsync(customer.Id);
                _notificationService.ShowSuccess("تم حذف العميل.");
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task OpenProfileAsync(Customer customer)
        {
            if (customer == null) return;
            var profileViewModel = new CustomerProfileViewModel(_customerService, _salesManagementService);
            profileViewModel.Initialize(customer);
            await _dialogService.ShowDialogAsync("CustomerProfileDialog", profileViewModel);
        }
    }
}
