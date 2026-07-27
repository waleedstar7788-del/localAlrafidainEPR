using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class CustomerSearchService : ICustomerSearchService
    {
        private readonly AppDbContext _context;

        public CustomerSearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> SearchCustomersAsync(string query, int page = 1, int pageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await _context.Customers
                    .AsNoTracking()
                    .OrderByDescending(c => c.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            query = query.ToLower();

            return await _context.Customers
                .AsNoTracking()
                .Where(c => c.FullName.ToLower().Contains(query) ||
                            c.Phone1.Contains(query) ||
                            c.CustomerNumber.Contains(query) ||
                            c.CompanyName.ToLower().Contains(query) ||
                            c.WhatsApp.Contains(query))
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Customer>> FilterCustomersAsync(CustomerType? type, bool? isActive, string? city = null)
        {
            var query = _context.Customers.AsNoTracking().AsQueryable();

            if (type.HasValue)
                query = query.Where(c => c.Type == type.Value);

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(c => c.City.ToLower() == city.ToLower());

            return await query.OrderByDescending(c => c.CreatedDate).ToListAsync();
        }
    }
}
