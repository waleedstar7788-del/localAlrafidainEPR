using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class ReturnStatisticsService : IReturnStatisticsService
    {
        private readonly AppDbContext _context;

        public ReturnStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetMonthlySalesReturnTotalAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await _context.SalesReturnInvoices
                .Where(r => r.ReturnDate >= startOfMonth && r.Status != ReturnStatus.Cancelled)
                .SumAsync(r => r.TotalRefundAmount);
        }

        public async Task<decimal> GetMonthlyPurchaseReturnTotalAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await _context.PurchaseReturnInvoices
                .Where(p => p.ReturnDate >= startOfMonth && p.Status != ReturnStatus.Cancelled)
                .SumAsync(p => p.TotalRefundAmount);
        }

        public async Task<int> GetTodayReturnsCountAsync()
        {
            var today = DateTime.Today;
            var salesCount = await _context.SalesReturnInvoices.Where(r => r.ReturnDate >= today).CountAsync();
            var purchaseCount = await _context.PurchaseReturnInvoices.Where(p => p.ReturnDate >= today).CountAsync();
            
            return salesCount + purchaseCount;
        }
    }
}
