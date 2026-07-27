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
    public partial class PurchaseReturnEditorViewModel : BaseViewModel
    {
        private readonly IPurchaseReturnService _returnService;
        private readonly ISupplierService _supplierService;
        private readonly INotificationService _notificationService;

        [ObservableProperty] private PurchaseReturnInvoice _returnInvoice;
        [ObservableProperty] private ObservableCollection<Supplier> _suppliers = new();
        [ObservableProperty] private Supplier? _selectedSupplier;

        public PurchaseReturnEditorViewModel(
            IPurchaseReturnService returnService,
            ISupplierService supplierService,
            INotificationService notificationService)
        {
            _returnService = returnService;
            _supplierService = supplierService;
            _notificationService = notificationService;

            ReturnInvoice = new PurchaseReturnInvoice 
            { 
                ReturnNumber = "PR-" + DateTime.Now.Ticks.ToString().Substring(10),
                ReturnDate = DateTime.Now,
                RefundMethod = RefundMethod.Credit // Default for suppliers
            };

            LoadSuppliersAsync().ConfigureAwait(false);
        }

        private async Task LoadSuppliersAsync()
        {
            var list = await _supplierService.GetSuppliersAsync(1, 1000);
            foreach (var s in list) Suppliers.Add(s);
        }

        partial void OnSelectedSupplierChanged(Supplier? value)
        {
            if (value != null) ReturnInvoice.SupplierId = value.Id;
        }

        [RelayCommand]
        private void AddItem()
        {
            ReturnInvoice.Items.Add(new PurchaseReturnItem { QuantityReturned = 1 });
            RecalculateTotals();
        }

        [RelayCommand]
        private void RemoveItem(PurchaseReturnItem item)
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
            if (SelectedSupplier == null)
            {
                _notificationService.ShowError("الرجاء تحديد المورد.");
                return;
            }

            if (!ReturnInvoice.Items.Any())
            {
                _notificationService.ShowError("لا توجد منتجات في المرتجع.");
                return;
            }

            RecalculateTotals();
            
            await _returnService.ProcessReturnAsync(ReturnInvoice);
            _notificationService.ShowSuccess("تم تنفيذ مرتجع المشتريات بنجاح.");
            
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
