using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class ReportingService : IReportingService
    {
        private readonly AppDbContext _context;

        public ReportingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime start, DateTime end)
        {
            return await _context.SalesInvoices
                .AsNoTracking()
                .Where(s => s.InvoiceDate >= start && s.InvoiceDate <= end && s.Status == SalesInvoiceStatus.Paid)
                .SumAsync(s => s.GrandTotal);
        }

        public async Task<decimal> GetTotalPurchasesAsync(DateTime start, DateTime end)
        {
            return await _context.PurchaseInvoices
                .AsNoTracking()
                .Where(p => p.InvoiceDate >= start && p.InvoiceDate <= end && p.Status == InvoiceStatus.Completed)
                .SumAsync(p => p.TotalAmount);
        }

        public async Task<decimal> GetTotalExpensesAsync(DateTime start, DateTime end)
        {
            return await _context.ExpenseEntries
                .AsNoTracking()
                .Where(e => e.ExpenseDate >= start && e.ExpenseDate <= end && e.Status == FinancialTransactionStatus.Approved)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> GetTotalIncomeAsync(DateTime start, DateTime end)
        {
            return await _context.IncomeEntries
                .AsNoTracking()
                .Where(i => i.IncomeDate >= start && i.IncomeDate <= end && i.Status == FinancialTransactionStatus.Approved)
                .SumAsync(i => i.Amount);
        }

        public async Task<decimal> GetInventoryValuationAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .SumAsync(p => p.StockQuantity * p.CostPrice);
        }

        public async Task<Dictionary<string, decimal>> GetTopSellingProductsAsync(int count)
        {
            var topProducts = await _context.SalesItems
                .AsNoTracking()
                .Include(si => si.Product)
                .Where(si => si.SalesInvoice.Status == SalesInvoiceStatus.Paid)
                .GroupBy(si => si.Product.Name)
                .Select(g => new { ProductName = g.Key, TotalSold = g.Sum(x => x.Quantity) })
                .OrderByDescending(g => g.TotalSold)
                .Take(count)
                .ToDictionaryAsync(g => g.ProductName, g => (decimal)g.TotalSold);

            return topProducts;
        }

        public async Task<Dictionary<DateTime, decimal>> GetSalesTrendAsync(int days)
        {
            var startDate = DateTime.Today.AddDays(-days);
            
            // We group by Date. EF Core Date property translation might vary by provider, 
            // but for SQLite, formatting Date is usually safe if simple.
            // A safer approach for SQLite is downloading records and grouping in memory if count is small, 
            // but for a true ERP, we'd use DB functions. We'll pull data for the last X days and group in memory to avoid EF SQLite date translation issues.

            var invoices = await _context.SalesInvoices
                .AsNoTracking()
                .Where(s => s.InvoiceDate >= startDate && s.Status == SalesInvoiceStatus.Paid)
                .Select(s => new { s.InvoiceDate, s.GrandTotal })
                .ToListAsync();

            var trend = invoices
                .GroupBy(i => i.InvoiceDate.Date)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.GrandTotal));

            // Fill missing days with 0
            var result = new Dictionary<DateTime, decimal>();
            for (int i = 0; i <= days; i++)
            {
                var d = startDate.AddDays(i).Date;
                result[d] = trend.ContainsKey(d) ? trend[d] : 0;
            }

            return result;
        }
    }
}
