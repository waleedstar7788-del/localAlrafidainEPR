using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class PurchaseSearchService : IPurchaseSearchService
    {
        private readonly AppDbContext _context;

        public PurchaseSearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseInvoice>> SearchInvoicesAsync(string query, int page = 1, int pageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await _context.PurchaseInvoices
                    .Include(p => p.Supplier)
                    .AsNoTracking()
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            query = query.ToLower();

            return await _context.PurchaseInvoices
                .Include(p => p.Supplier)
                .AsNoTracking()
                .Where(p => p.InvoiceNumber.ToLower().Contains(query) ||
                            p.Supplier.SupplierName.ToLower().Contains(query) ||
                            p.ReferenceNumber.ToLower().Contains(query))
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
