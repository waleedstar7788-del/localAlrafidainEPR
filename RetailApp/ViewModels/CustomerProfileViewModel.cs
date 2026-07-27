using CommunityToolkit.Mvvm.ComponentModel;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.ViewModels
{
    public partial class CustomerProfileViewModel : BaseViewModel
    {
        private readonly ICustomerService _customerService;
        private readonly ISalesManagementService _salesManagementService;
        
        [ObservableProperty]
        private Customer _customer = null!;

        [ObservableProperty]
        private System.Collections.ObjectModel.ObservableCollection<SalesInvoice> _invoices = new();

        [ObservableProperty]
        private bool _hasInvoices;

        [ObservableProperty]
        private bool _noInvoices = true;

        public CustomerProfileViewModel(ICustomerService customerService, ISalesManagementService salesManagementService)
        {
            _customerService = customerService;
            _salesManagementService = salesManagementService;
        }

        public async void Initialize(Customer customer)
        {
            Customer = customer;
            await LoadInvoicesAsync();
        }

        private async System.Threading.Tasks.Task LoadInvoicesAsync()
        {
            var list = await _salesManagementService.GetInvoicesByCustomerIdAsync(Customer.Id);
            Invoices.Clear();
            foreach (var inv in list) Invoices.Add(inv);
            HasInvoices = Invoices.Count > 0;
            NoInvoices = !HasInvoices;
        }
    }
}
