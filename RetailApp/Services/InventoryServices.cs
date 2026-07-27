using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class MockInventoryService : IInventoryService
    {
        public Task<bool> AdjustStockAsync(int productId, int quantityChange, string reason, string user) => Task.FromResult(true);
        public Task<int> GetAvailableStockAsync(int productId) => Task.FromResult(50);
        public Task<int> GetCurrentStockAsync(int productId) => Task.FromResult(60);
        public Task<int> GetIncomingStockAsync(int productId) => Task.FromResult(100);
        public Task<int> GetOutgoingStockAsync(int productId) => Task.FromResult(10);
        public Task<int> GetReservedStockAsync(int productId) => Task.FromResult(10);
    }

    public class MockCategoryService : ICategoryService
    {
        public Task<bool> DeleteCategoryAsync(int id) => Task.FromResult(true);
        public Task<List<Category>> GetAllCategoriesAsync() => Task.FromResult(new List<Category> {
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Clothing" }
        });
        public Task<Category?> GetCategoryByIdAsync(int id) => Task.FromResult<Category?>(null);
        public Task<bool> SaveCategoryAsync(Category category) => Task.FromResult(true);
    }

    public class MockBrandService : IBrandService
    {
        public Task<List<Brand>> GetAllBrandsAsync() => Task.FromResult(new List<Brand> {
            new Brand { Id = 1, Name = "Apple" },
            new Brand { Id = 2, Name = "Samsung" }
        });
        public Task<bool> SaveBrandAsync(Brand brand) => Task.FromResult(true);
    }

    public class MockBarcodeService : IBarcodeService
    {
        public Task<string> GenerateUniqueBarcodeAsync() => Task.FromResult(new Random().Next(10000000, 99999999).ToString());
        public Task<bool> ValidateBarcodeIsUniqueAsync(string barcode, int currentProductId = 0) => Task.FromResult(true);
    }

    public class MockStockMovementService : IStockMovementService
    {
        public Task<List<StockMovement>> GetMovementsByProductAsync(int productId) => Task.FromResult(new List<StockMovement>());
        public Task<bool> LogMovementAsync(StockMovement movement) => Task.FromResult(true);
    }
}
