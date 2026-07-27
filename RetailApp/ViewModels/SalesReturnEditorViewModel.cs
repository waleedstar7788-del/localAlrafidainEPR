using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class SalesReturnEditorViewModel : BaseViewModel
    {
        private readonly ISalesReturnService _returnService;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;

        [ObservableProperty] private SalesReturnInvoice _returnInvoice = null!;
        [ObservableProperty] private ObservableCollection<Customer> _customers = new();
        [ObservableProperty] private Customer? _selectedCustomer;

        public SalesReturnEditorViewModel(
            ISalesReturnService returnService,
            ICustomerService customerService,
            INotificationService notificationService)
        {
            _returnService = returnService;
            _customerService = customerService;
            _notificationService = notificationService;

            ReturnInvoice = new SalesReturnInvoice 
            { 
                ReturnNumber = "SR-" + DateTime.Now.Ticks.ToString().Substring(10),
                ReturnDate = DateTime.Now
            };

            LoadCustomersAsync().ConfigureAwait(false);
        }

        private async Task LoadCustomersAsync()
        {
            var list = await _customerService.GetCustomersAsync(1, 1000);
            foreach (var c in list) Customers.Add(c);
        }

        partial void OnSelectedCustomerChanged(Customer? value)
        {
            if (value != null) ReturnInvoice.CustomerId = value.Id;
        }

        [RelayCommand]
        private void AddItem()
        {
            ReturnInvoice.Items.Add(new SalesReturnItem { QuantityReturned = 1 });
            RecalculateTotals();
        }

        [RelayCommand]
        private void RemoveItem(SalesReturnItem item)
        {
            if (item != null && ReturnInvoice.Items.Contains(item))
            {
                ReturnInvoice.Items.Remove(item);
                RecalculateTotals();
            }
        }

        public void RecalculateTotals()
        {
            ReturnInvoice.TotalRefundAmount = ReturnInvoice.Items.Sum(i => i.SubTotal);
            OnPropertyChanged(nameof(ReturnInvoice));
        }

        [RelayCommand]
        private async Task ProcessReturnAsync(object? windowInstance)
        {
            if (!ReturnInvoice.Items.Any())
            {
                _notificationService.ShowError("لا توجد منتجات في المرتجع.");
                return;
            }

            RecalculateTotals();
            
            await _returnService.ProcessReturnAsync(ReturnInvoice);
            _notificationService.ShowSuccess("تم تنفيذ المرتجع بنجاح.");
            
            if (windowInstance is System.Windows.Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        [RelayCommand]
        private void Cancel(object? windowInstance)
        {
            if (windowInstance is System.Windows.Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
