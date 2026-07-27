using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class SupplierSearchService : ISupplierSearchService
    {
        private readonly AppDbContext _context;

        public SupplierSearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> SearchSuppliersAsync(string query, int page = 1, int pageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await _context.Suppliers
                    .AsNoTracking()
                    .OrderByDescending(s => s.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            query = query.ToLower();

            return await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.SupplierName.ToLower().Contains(query) ||
                            s.Phone1.Contains(query) ||
                            s.SupplierNumber.Contains(query) ||
                            s.CompanyName.ToLower().Contains(query) ||
                            s.TaxNumber.Contains(query))
                .OrderByDescending(s => s.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
