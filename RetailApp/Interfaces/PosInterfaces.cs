using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetProductsByCategoryAsync(string category);
        Task<List<Product>> SearchProductsAsync(string query);
        Task<List<Category>> GetCategoriesAsync();
        Task<Product?> GetProductByBarcodeAsync(string barcode);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
    }

    public interface IInvoiceService
    {
        Task<string> GenerateNextInvoiceNumberAsync();
        Task<bool> SaveInvoiceAsync(Invoice invoice);
        Task<bool> HoldInvoiceAsync(Invoice invoice);
        Task<List<Invoice>> GetHeldInvoicesAsync();
    }

    public interface ISalesService
    {
        Task<bool> ValidateSaleAsync(Invoice invoice);
        Task<Invoice> CompleteSaleAsync(Invoice invoice);
    }

    public interface IReceiptService
    {
        Task PrintThermalReceiptAsync(Invoice invoice);
        Task PrintA4InvoiceAsync(Invoice invoice);
    }

    public interface IPricingService
    {
        Task<decimal> CalculateTaxAsync(decimal subTotal);
        Task<decimal> CalculateDiscountAsync(decimal subTotal, string discountType, decimal discountValue);
    }
}
