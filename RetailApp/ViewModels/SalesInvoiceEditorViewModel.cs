using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RetailApp.ViewModels
{
    public partial class SalesInvoiceEditorViewModel : ObservableObject
    {
        private readonly ISalesManagementService _salesService;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private SalesInvoice _invoice = new();

        [ObservableProperty]
        private ObservableCollection<SalesItem> _items = new();

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Product> _searchResults = new();

        [ObservableProperty]
        private bool _hasSearchResults;

        [ObservableProperty]
        private decimal _subTotal;

        [ObservableProperty]
        private decimal _taxAmount;

        [ObservableProperty]
        private decimal _discountAmount;

        [ObservableProperty]
        private decimal _grandTotal;

        [ObservableProperty]
        private string _notes = string.Empty;

        public bool IsEditMode { get; private set; }

        public SalesInvoiceEditorViewModel(
            ISalesManagementService salesService,
            IProductService productService,
            INotificationService notificationService,
            SalesInvoice? existingInvoice = null)
        {
            _salesService = salesService;
            _productService = productService;
            _notificationService = notificationService;

            if (existingInvoice != null)
            {
                IsEditMode = true;
                Invoice = existingInvoice;
                Notes = existingInvoice.Notes;
                foreach (var item in existingInvoice.Items)
                {
                    Items.Add(item);
                }
                RecalculateTotals();
            }
        }

        partial void OnSearchQueryChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                SearchResults.Clear();
                HasSearchResults = false;
                return;
            }
            _ = PerformSearchAsync(value);
        }

        private async Task PerformSearchAsync(string query)
        {
            SearchResults.Clear();
            var results = await _productService.SearchProductsAsync(query);
            foreach (var p in results) SearchResults.Add(p);
            HasSearchResults = SearchResults.Count > 0;
        }

        [RelayCommand]
        private void AddProduct(Product product)
        {
            if (product == null) return;

            var existing = Items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existing != null)
            {
                existing.Quantity++;
                existing.SubTotal = (existing.Quantity * existing.UnitPrice) - existing.Discount;
            }
            else
            {
                var newItem = new SalesItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    UnitCost = product.CostPrice,
                    Discount = 0,
                    SubTotal = product.Price
                };
                Items.Add(newItem);
            }

            SearchQuery = string.Empty;
            SearchResults.Clear();
            HasSearchResults = false;
            RecalculateTotals();
        }

        [RelayCommand]
        private void RemoveItem(SalesItem item)
        {
            if (item != null)
            {
                Items.Remove(item);
                RecalculateTotals();
            }
        }

        [RelayCommand]
        private void RecalculateTotals()
        {
            foreach (var item in Items)
            {
                item.SubTotal = (item.Quantity * item.UnitPrice) - item.Discount;
            }

            SubTotal = Items.Sum(i => i.SubTotal);
            TaxAmount = Invoice.TaxAmount;
            DiscountAmount = Invoice.DiscountAmount;
            GrandTotal = SubTotal + TaxAmount - DiscountAmount;
        }

        [RelayCommand]
        private async Task SaveAsync(Window window)
        {
            try
            {
                // Update the invoice object
                Invoice.Items = Items.ToList();
                Invoice.SubTotal = SubTotal;
                Invoice.TaxAmount = TaxAmount;
                Invoice.DiscountAmount = DiscountAmount;
                Invoice.GrandTotal = GrandTotal;
                Invoice.RemainingAmount = GrandTotal - Invoice.PaidAmount;
                Invoice.Notes = Notes;

                // Recalculate profit
                Invoice.TotalCost = Items.Sum(i => i.Quantity * i.UnitCost);
                Invoice.GrossProfit = Invoice.SubTotal - Invoice.TotalCost;
                Invoice.NetProfit = Invoice.GrandTotal - Invoice.TotalCost;

                await _salesService.UpdateInvoiceAsync(Invoice);

                _notificationService.ShowSuccess("تم حفظ تعديلات الفاتورة بنجاح!");
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"خطأ أثناء الحفظ: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
