using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class MockProductService : IProductService
    {
        private static readonly List<Category> _categories = new List<Category>
        {
            new Category { Id = 1, Name = "All", Icon = "ViewAll" },
            new Category { Id = 2, Name = "Electronics", Icon = "Laptop" },
            new Category { Id = 3, Name = "Clothing", Icon = "TshirtCrew" },
            new Category { Id = 4, Name = "Groceries", Icon = "FoodApple" }
        };

        private static readonly List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop HP", Barcode = "1001", SellingPrice = 1200m, CurrentQuantity = 10, Category = "Electronics", ImageUrl = "laptop.png" },
            new Product { Id = 2, Name = "Wireless Mouse", Barcode = "1002", SellingPrice = 25m, CurrentQuantity = 50, Category = "Electronics", ImageUrl = "mouse.png" },
            new Product { Id = 3, Name = "Mechanical Keyboard", Barcode = "1003", SellingPrice = 80m, CurrentQuantity = 20, Category = "Electronics", ImageUrl = "keyboard.png" },
            new Product { Id = 4, Name = "Monitor 24 inch", Barcode = "1004", SellingPrice = 150m, CurrentQuantity = 15, Category = "Electronics", ImageUrl = "monitor.png" },
            new Product { Id = 5, Name = "USB-C Cable", Barcode = "1005", SellingPrice = 15m, CurrentQuantity = 100, Category = "Accessories", ImageUrl = "cable.png" }
        };

        public MockProductService()
        {
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            await Task.Delay(100);
            return _categories;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            await Task.Delay(200);
            return _products;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            await Task.Delay(150);
            if (category == "All") return _products;
            return _products.Where(p => p.Category == category).ToList();
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            await Task.Delay(150);
            var lowerQuery = query.ToLower();
            return _products.Where(p => p.Name.ToLower().Contains(lowerQuery) || p.SKU.ToLower().Contains(lowerQuery) || p.Barcode.Contains(query)).ToList();
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            await Task.Delay(100);
            return _products.FirstOrDefault(p => p.Barcode == barcode);
        }

        public async Task AddProductAsync(Product product)
        {
            await Task.Delay(200);
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await Task.Delay(200);
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                var index = _products.IndexOf(existing);
                _products[index] = product;
            }
        }
    }

    public class MockInvoiceService : IInvoiceService
    {
        public async Task<string> GenerateNextInvoiceNumberAsync()
        {
            await Task.Delay(50);
            return $"INV-{System.DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }

        public async Task<List<Invoice>> GetHeldInvoicesAsync()
        {
            await Task.Delay(100);
            return new List<Invoice>();
        }

        public async Task<bool> HoldInvoiceAsync(Invoice invoice)
        {
            await Task.Delay(200);
            return true;
        }

        public async Task<bool> SaveInvoiceAsync(Invoice invoice)
        {
            await Task.Delay(300);
            return true;
        }
    }

    public class MockSalesService : ISalesService
    {
        public async Task<Invoice> CompleteSaleAsync(Invoice invoice)
        {
            await Task.Delay(500);
            return invoice;
        }

        public async Task<bool> ValidateSaleAsync(Invoice invoice)
        {
            await Task.Delay(100);
            return invoice.Items.Any() && !invoice.Items.Any(i => i.Quantity <= 0);
        }
    }

    public class MockPricingService : IPricingService
    {
        public Task<decimal> CalculateDiscountAsync(decimal subTotal, string discountType, decimal discountValue)
        {
            if (discountType == "Percentage") return Task.FromResult(subTotal * (discountValue / 100m));
            return Task.FromResult(discountValue);
        }

        public Task<decimal> CalculateTaxAsync(decimal subTotal)
        {
            return Task.FromResult(subTotal * 0.15m); // 15% VAT placeholder
        }
    }
}
