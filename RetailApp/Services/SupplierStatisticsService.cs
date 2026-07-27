using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class SupplierStatisticsService : ISupplierStatisticsService
    {
        private readonly AppDbContext _context;

        public SupplierStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalSuppliersAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        public async Task<int> GetActiveSuppliersAsync()
        {
            return await _context.Suppliers.Where(s => s.IsActive).CountAsync();
        }

        public async Task<int> GetSuppliersWithOutstandingBalanceAsync()
        {
            return await _context.Suppliers.Where(s => s.CurrentBalance > 0).CountAsync();
        }

        public async Task<int> GetNewSuppliersThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await _context.Suppliers.Where(s => s.RegistrationDate >= startOfMonth).CountAsync();
        }
    }
}
