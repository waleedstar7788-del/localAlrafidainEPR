using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class CustomerStatisticsService : ICustomerStatisticsService
    {
        private readonly AppDbContext _context;

        public CustomerStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<int> GetNewCustomersThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await _context.Customers
                .Where(c => c.CreatedDate >= startOfMonth)
                .CountAsync();
        }

        public async Task<int> GetVipCustomersCountAsync()
        {
            return await _context.Customers
                .Where(c => c.Rank == CustomerRank.VIP)
                .CountAsync();
        }

        public async Task<int> GetCustomersWithDebtCountAsync()
        {
            return await _context.Customers
                .Where(c => c.CurrentBalance < 0) // Assuming negative means they owe us
                .CountAsync();
        }
    }
}
