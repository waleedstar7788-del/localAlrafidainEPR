using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            // For now, return dynamic categories based on products, or a static list.
            // A real app would query _context.Categories.
            return await Task.FromResult(new List<Category>
            {
                new Category { Id = 1, Name = "All", Icon = "ViewAll" },
                new Category { Id = 2, Name = "Electronics", Icon = "Laptop" },
                new Category { Id = 3, Name = "Clothing", Icon = "TshirtCrew" },
                new Category { Id = 4, Name = "Groceries", Icon = "FoodApple" }
            });
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return null;
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            if (category == "All")
            {
                return await GetAllProductsAsync();
            }
            return await _context.Products.AsNoTracking().Where(p => p.Category == category).ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return await GetAllProductsAsync();
            var q = query.ToLower();
            return await _context.Products.AsNoTracking().Where(p => 
                (p.Name != null && p.Name.ToLower().Contains(q)) || 
                (p.Barcode != null && p.Barcode.Contains(q))
            ).ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
