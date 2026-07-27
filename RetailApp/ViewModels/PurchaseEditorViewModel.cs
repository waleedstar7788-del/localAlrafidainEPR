using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RetailApp.ViewModels
{
    public partial class PurchaseEditorViewModel : BaseViewModel
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IPurchaseCalculationService _calculationService;
        private readonly ISupplierService _supplierService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private PurchaseInvoice _invoice;

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers = new();

        [ObservableProperty]
        private Supplier? _selectedSupplier;

        public PurchaseEditorViewModel(
            IPurchaseService purchaseService,
            IPurchaseCalculationService calculationService,
            ISupplierService supplierService,
            INotificationService notificationService,
            PurchaseInvoice? existingInvoice = null)
        {
            _purchaseService = purchaseService;
            _calculationService = calculationService;
            _supplierService = supplierService;
            _notificationService = notificationService;

            if (existingInvoice == null)
            {
                Invoice = new PurchaseInvoice();
                InitializeNewInvoiceAsync().ConfigureAwait(false);
            }
            else
            {
                Invoice = existingInvoice;
            }

            LoadSuppliersAsync().ConfigureAwait(false);
        }

        private async Task InitializeNewInvoiceAsync()
        {
            Invoice.InvoiceNumber = await _purchaseService.GenerateNextInvoiceNumberAsync();
            OnPropertyChanged(nameof(Invoice));
        }

        private async Task LoadSuppliersAsync()
        {
            var list = await _supplierService.GetSuppliersAsync(1, 1000); // Load active
            foreach (var s in list) Suppliers.Add(s);

            if (Invoice.SupplierId > 0)
            {
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == Invoice.SupplierId);
            }
        }

        partial void OnSelectedSupplierChanged(Supplier? value)
        {
            if (value != null)
            {
                Invoice.SupplierId = value.Id;
            }
        }

        [RelayCommand]
        private void AddItem()
        {
            Invoice.Items.Add(new PurchaseItem { Quantity = 1 });
            RecalculateTotals();
        }

        [RelayCommand]
        private void RemoveItem(PurchaseItem item)
        {
            if (item != null && Invoice.Items.Contains(item))
            {
                Invoice.Items.Remove(item);
                RecalculateTotals();
            }
        }

        public void RecalculateTotals()
        {
            foreach (var item in Invoice.Items)
            {
                item.SubTotal = (item.Quantity * item.PurchasePrice) - item.Discount + item.Tax;
            }

            Invoice.SubTotal = Invoice.Items.Sum(i => i.SubTotal);
            Invoice.TotalAmount = Invoice.SubTotal - Invoice.DiscountAmount + Invoice.TaxAmount;
            
            OnPropertyChanged(nameof(Invoice));
        }

        [RelayCommand]
        private async Task SaveDraftAsync(object? windowInstance)
        {
            RecalculateTotals();
            Invoice.Status = InvoiceStatus.Draft;
            
            if (Invoice.Id == 0)
                await _purchaseService.AddInvoiceAsync(Invoice);
            else
                await _purchaseService.UpdateInvoiceAsync(Invoice);

            _notificationService.ShowSuccess("تم حفظ الفاتورة كمسودة.");

            if (windowInstance is System.Windows.Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        [RelayCommand]
        private async Task CompletePurchaseAsync(object? windowInstance)
        {
            RecalculateTotals();
            Invoice.Status = InvoiceStatus.Completed;

            if (Invoice.Id == 0)
                await _purchaseService.AddInvoiceAsync(Invoice);
            else
                await _purchaseService.UpdateInvoiceAsync(Invoice);

            // Trigger Inventory & Supplier updates
            await _calculationService.ProcessCompletedPurchaseAsync(Invoice);

            _notificationService.ShowSuccess("تم استكمال فاتورة المشتريات وإضافة الكميات للمخزن.");

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
