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
    public partial class PosViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IInvoiceService _invoiceService;
        private readonly ISalesService _salesService;
        private readonly IPricingService _pricingService;
        private readonly INavigationService _navigationService;
        private readonly MainViewModel _mainViewModel;
        private readonly ICustomerService _customerService;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _searchQuery = string.Empty;
        [ObservableProperty] private Invoice _currentInvoice = new Invoice();
        [ObservableProperty] private string _selectedCategory = "All";
        [ObservableProperty] private Customer? _selectedCustomer;

        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();
        public ObservableCollection<Customer> Customers { get; } = new ObservableCollection<Customer>();

        public PosViewModel(
            IProductService productService, 
            IInvoiceService invoiceService, 
            ISalesService salesService, 
            IPricingService pricingService,
            INavigationService navigationService,
            MainViewModel mainViewModel,
            ICustomerService customerService,
            IDialogService dialogService)
        {
            _productService = productService;
            _invoiceService = invoiceService;
            _salesService = salesService;
            _pricingService = pricingService;
            _navigationService = navigationService;
            _mainViewModel = mainViewModel;
            _customerService = customerService;
            _dialogService = dialogService;

            AttachCartItemEvents();
            _ = InitializeAsync();
        }

        private void AttachCartItemEvents()
        {
            CurrentInvoice.Items.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (CartItem item in e.NewItems)
                    {
                        item.PropertyChanged += CartItem_PropertyChanged;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (CartItem item in e.OldItems)
                    {
                        item.PropertyChanged -= CartItem_PropertyChanged;
                    }
                }
                _ = RecalculateTotalsAsync();
            };
        }

        private void CartItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.Quantity) || 
                e.PropertyName == nameof(CartItem.UnitPrice) || 
                e.PropertyName == nameof(CartItem.Discount))
            {
                _ = RecalculateTotalsAsync();
            }
        }

        partial void OnSelectedCustomerChanged(Customer? value)
        {
            CurrentInvoice.Customer = value;
        }

        public bool HasSearchResults => Products.Count > 0;

        private async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                CurrentInvoice.InvoiceNumber = await _invoiceService.GenerateNextInvoiceNumberAsync();
                CurrentInvoice.Date = DateTime.Now;
                CurrentInvoice.Cashier = "Admin";
                OnPropertyChanged(nameof(CurrentInvoice));

                var cats = await _productService.GetCategoriesAsync();
                foreach(var c in cats) Categories.Add(c);

                // We don't want to load all products if we don't have a side panel.
                // We'll leave Products empty until they search.
                // await LoadProductsAsync();
                await LoadCustomersAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCustomersAsync()
        {
            Customers.Clear();
            
            // Add Cash Customer at the top
            var cashCustomer = new Customer { Id = 0, FullName = "عميل نقدي" };
            Customers.Add(cashCustomer);
            
            var items = await _customerService.GetCustomersAsync(1, 1000);
            foreach (var item in items) Customers.Add(item);

            if (SelectedCustomer == null)
            {
                SelectedCustomer = cashCustomer;
            }
        }

        private async Task LoadProductsAsync()
        {
            Products.Clear();
            var items = await _productService.GetProductsByCategoryAsync(SelectedCategory);
            foreach(var item in items) Products.Add(item);
        }

        [RelayCommand]
        private async Task FilterByCategoryAsync(string category)
        {
            SelectedCategory = category;
            await LoadProductsAsync();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            Products.Clear();
            OnPropertyChanged(nameof(HasSearchResults));
            
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                return;
            }

            // Check if it's an exact barcode match first
            var exactMatch = await _productService.GetProductByBarcodeAsync(SearchQuery);
            if (exactMatch != null)
            {
                AddToCart(exactMatch);
                SearchQuery = string.Empty; // clear
                return;
            }

            var results = await _productService.SearchProductsAsync(SearchQuery);
            foreach(var r in results) Products.Add(r);
            OnPropertyChanged(nameof(HasSearchResults));
        }

        partial void OnSearchQueryChanged(string value)
        {
            _ = SearchAsync();
        }

        [RelayCommand]
        private void AddToCart(Product product)
        {
            if (product.StockQuantity <= 0) return; // Prevent negative stock

            var existing = CurrentInvoice.Items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                CurrentInvoice.Items.Add(new CartItem { Product = product, Quantity = 1, UnitPrice = product.Price });
            }
            // Recalculation is now handled by the CollectionChanged and PropertyChanged events
        }

        [RelayCommand]
        private void RemoveFromCart(CartItem item)
        {
            CurrentInvoice.Items.Remove(item);
            // Recalculation is handled by events
        }

        [RelayCommand]
        private void IncreaseQuantity(CartItem item)
        {
            item.Quantity++;
        }

        [RelayCommand]
        private void DecreaseQuantity(CartItem item)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
        }

        [RelayCommand]
        private void ClearCart()
        {
            CurrentInvoice.Items.Clear();
            _ = RecalculateTotalsAsync(); // Called explicitly here because CollectionChanged won't fire individual property changes
        }

        private async Task RecalculateTotalsAsync()
        {
            CurrentInvoice.SubTotal = CurrentInvoice.Items.Sum(i => i.Total);
            CurrentInvoice.Tax = await _pricingService.CalculateTaxAsync(CurrentInvoice.SubTotal);
            CurrentInvoice.GrandTotal = CurrentInvoice.SubTotal + CurrentInvoice.Tax - CurrentInvoice.Discount;
            
            // Trigger UI update
            OnPropertyChanged(nameof(CurrentInvoice));
        }

        // --- Keyboard Shortcuts ---
        [RelayCommand] private void ShortcutSearch() { /* Focus logic handled in view */ }
        [RelayCommand] private void ShortcutCustomer() { _ = AddNewCustomerAsync(); }

        [RelayCommand]
        private async Task AddNewCustomerAsync()
        {
            var editorViewModel = new CustomerEditorViewModel(_customerService, null);
            var result = await _dialogService.ShowDialogAsync("CustomerEditorDialog", editorViewModel);
            if (result == true)
            {
                await LoadCustomersAsync();
                SelectedCustomer = Customers.LastOrDefault(); // Select the newly added customer
            }
        }

        [RelayCommand] private async Task ShortcutHoldAsync() { await _invoiceService.HoldInvoiceAsync(CurrentInvoice); _mainViewModel.NavigateToDashboardCommand.Execute(null); }
        [RelayCommand] private void ShortcutPayment() { /* Open payment modal */ }
        [RelayCommand] private async Task ShortcutCompleteAsync() 
        {
            if (await _salesService.ValidateSaleAsync(CurrentInvoice))
            {
                await _salesService.CompleteSaleAsync(CurrentInvoice);
                _mainViewModel.NavigateToDashboardCommand.Execute(null);
            }
        }
        [RelayCommand] private void ShortcutPrint() { /* Print receipt */ }
        [RelayCommand] private void ShortcutCancel() { _mainViewModel.NavigateToDashboardCommand.Execute(null); }
    }
}
