using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace RetailApp.ViewModels
{
    public partial class ProductEditorViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IBarcodeService _barcodeService;
        private readonly ICategoryService _categoryService;

        [ObservableProperty] private Product _editingProduct = new Product();
        [ObservableProperty] private bool _isEditMode;
        
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        public ProductEditorViewModel(IProductService productService, IBarcodeService barcodeService, ICategoryService categoryService)
        {
            _productService = productService;
            _barcodeService = barcodeService;
            _categoryService = categoryService;
        }

        public async Task LoadProductAsync(Product? product = null)
        {
            Categories.Clear();
            var cats = await _categoryService.GetAllCategoriesAsync();
            foreach (var c in cats) Categories.Add(c);

            if (product != null)
            {
                EditingProduct = product;
                IsEditMode = true;
            }
            else
            {
                EditingProduct = new Product { Barcode = await _barcodeService.GenerateUniqueBarcodeAsync() };
                IsEditMode = false;
            }
        }

        public Action? OnRequestClose { get; set; }

        [RelayCommand]
        private void Cancel(object? windowInstance)
        {
            if (windowInstance is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
            else
            {
                OnRequestClose?.Invoke();
            }
        }

        [RelayCommand]
        private async Task SaveAsync(object? windowInstance)
        {
            // validation logic
            if (string.IsNullOrWhiteSpace(EditingProduct.Name) && string.IsNullOrWhiteSpace(EditingProduct.ArabicName)) return;
            
            if (IsEditMode)
            {
                await _productService.UpdateProductAsync(EditingProduct);
            }
            else
            {
                await _productService.AddProductAsync(EditingProduct);
            }
            
            if (windowInstance is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
            else
            {
                OnRequestClose?.Invoke();
            }
        }
    }
}
