using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class PurchaseStatisticsService : IPurchaseStatisticsService
    {
        private readonly AppDbContext _context;

        public PurchaseStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTodayPurchasesCountAsync()
        {
            var today = DateTime.Today;
            return await _context.PurchaseInvoices
                .Where(p => p.InvoiceDate >= today)
                .CountAsync();
        }

        public async Task<decimal> GetMonthlyPurchasesValueAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await _context.PurchaseInvoices
                .Where(p => p.InvoiceDate >= startOfMonth && p.Status == InvoiceStatus.Completed)
                .SumAsync(p => p.TotalAmount);
        }

        public async Task<decimal> GetOutstandingPurchasesValueAsync()
        {
            return await _context.PurchaseInvoices
                .Where(p => p.Status == InvoiceStatus.Completed)
                .SumAsync(p => p.TotalAmount - p.PaidAmount);
        }
    }
}
