using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IInventoryService
    {
        Task<int> GetCurrentStockAsync(int productId);
        Task<int> GetReservedStockAsync(int productId);
        Task<int> GetAvailableStockAsync(int productId);
        Task<int> GetIncomingStockAsync(int productId);
        Task<int> GetOutgoingStockAsync(int productId);
        Task<bool> AdjustStockAsync(int productId, int quantityChange, string reason, string user);
    }

    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<bool> SaveCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
    }

    public interface IBrandService
    {
        Task<List<Brand>> GetAllBrandsAsync();
        Task<bool> SaveBrandAsync(Brand brand);
    }

    public interface IBarcodeService
    {
        Task<string> GenerateUniqueBarcodeAsync();
        Task<bool> ValidateBarcodeIsUniqueAsync(string barcode, int currentProductId = 0);
    }

    public interface IStockMovementService
    {
        Task<List<StockMovement>> GetMovementsByProductAsync(int productId);
        Task<bool> LogMovementAsync(StockMovement movement);
    }
}
