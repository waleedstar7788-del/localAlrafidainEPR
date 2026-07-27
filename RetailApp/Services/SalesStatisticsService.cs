using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class SalesStatisticsService : ISalesStatisticsService
    {
        private readonly AppDbContext _context;

        public SalesStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTodaySalesTotalAsync()
        {
            var today = DateTime.Today;
            return await _context.SalesInvoices
                .Where(s => s.InvoiceDate >= today && s.Status != SalesInvoiceStatus.Cancelled)
                .SumAsync(s => s.GrandTotal);
        }

        public async Task<decimal> GetMonthlySalesTotalAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await _context.SalesInvoices
                .Where(s => s.InvoiceDate >= startOfMonth && s.Status != SalesInvoiceStatus.Cancelled)
                .SumAsync(s => s.GrandTotal);
        }

        public async Task<decimal> GetTodayProfitAsync()
        {
            var today = DateTime.Today;
            return await _context.SalesInvoices
                .Where(s => s.InvoiceDate >= today && s.Status != SalesInvoiceStatus.Cancelled)
                .SumAsync(s => s.NetProfit);
        }

        public async Task<decimal> GetMonthlyProfitAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return await _context.SalesInvoices
                .Where(s => s.InvoiceDate >= startOfMonth && s.Status != SalesInvoiceStatus.Cancelled)
                .SumAsync(s => s.NetProfit);
        }
    }
}
