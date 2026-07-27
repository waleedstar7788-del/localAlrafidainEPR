using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class InventoryViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;
        
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _searchQuery = string.Empty;
        [ObservableProperty] private Product? _selectedProduct;
        [ObservableProperty] private bool _isEditorOpen;

        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        
        public ProductEditorViewModel EditorViewModel { get; }

        public InventoryViewModel(IProductService productService, IInventoryService inventoryService, ProductEditorViewModel editorViewModel)
        {
            _productService = productService;
            _inventoryService = inventoryService;
            EditorViewModel = editorViewModel;
            EditorViewModel.OnRequestClose = () => CloseEditor();
            
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                Products.Clear();
                var items = await _productService.GetAllProductsAsync();
                foreach(var item in items) Products.Add(item);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            IsLoading = true;
            Products.Clear();
            var items = await _productService.SearchProductsAsync(SearchQuery);
            foreach(var item in items) Products.Add(item);
            IsLoading = false;
        }

        partial void OnSearchQueryChanged(string value)
        {
            _ = SearchAsync();
        }

        [RelayCommand]
        private async Task OpenEditorAsync(Product? product)
        {
            await EditorViewModel.LoadProductAsync(product);
            IsEditorOpen = true;
        }

        [RelayCommand]
        private void CloseEditor()
        {
            IsEditorOpen = false;
            _ = LoadDataAsync();
        }

        [RelayCommand]
        private void DeleteProduct(Product product)
        {
            if (product != null)
            {
                Products.Remove(product);
                // Also remove from mock service
            }
        }
    }
}
