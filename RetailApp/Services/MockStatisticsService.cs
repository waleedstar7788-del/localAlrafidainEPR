using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class MockStatisticsService : IStatisticsService
    {
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;

        public MockStatisticsService(IProductService productService, IInventoryService inventoryService)
        {
            _productService = productService;
            _inventoryService = inventoryService;
        }

        public async Task<List<StatItem>> GetDashboardStatsAsync()
        {
            await Task.Delay(500); // Simulate network/DB latency
            
            var products = await _productService.GetAllProductsAsync();
            int totalProducts = products.Count;
            int outOfStock = 0;
            int lowStock = 0;
            decimal totalValue = 0;

            foreach(var p in products)
            {
                var stock = await _inventoryService.GetCurrentStockAsync(p.Id);
                if (stock <= 0) outOfStock++;
                else if (stock <= p.MinimumStock) lowStock++;
                
                totalValue += stock * p.PurchasePrice;
            }

            return new List<StatItem>
            {
                new StatItem { Title = "Today's Sales", Value = "12,450.00 د.ع", Icon = "CartOutline", Trend = "+5.2%", TrendIsPositive = true },
                new StatItem { Title = "Today's Purchases", Value = "3,210.00 د.ع", Icon = "BasketOutline", Trend = "-1.2%", TrendIsPositive = false },
                new StatItem { Title = "Today's Profit", Value = "4,100.00 د.ع", Icon = "CashRegister", Trend = "+8.4%", TrendIsPositive = true },
                new StatItem { Title = "Monthly Sales", Value = "210,400.00 د.ع", Icon = "CalendarMonth", Trend = "+12.5%", TrendIsPositive = true },
                new StatItem { Title = "Monthly Profit", Value = "45,200.00 د.ع", Icon = "Finance", Trend = "+15.0%", TrendIsPositive = true },
                new StatItem { Title = "Out Of Stock", Value = "0", Icon = "AlertCircleOutline", Trend = "-1", TrendIsPositive = true },
                new StatItem { Title = "Low Stock Products", Value = "0", Icon = "AlertOutline", Trend = "+2", TrendIsPositive = false },
                new StatItem { Title = "Inventory Value", Value = "0.00 د.ع", Icon = "Warehouse", Trend = "0", TrendIsPositive = true },
                new StatItem { Title = "Total Products", Value = "5", Icon = "PackageVariantClosed", Trend = "+12", TrendIsPositive = true },
                new StatItem { Title = "Total Customers", Value = "1,450", Icon = "AccountGroupOutline", Trend = "+45", TrendIsPositive = true },
                new StatItem { Title = "Cash Balance", Value = "145,000.00 د.ع", Icon = "WalletOutline", Trend = "+12,000 د.ع", TrendIsPositive = true },
                new StatItem { Title = "Pending Installments", Value = "18,500.00 د.ع", Icon = "ClockOutline", Trend = "-500 د.ع", TrendIsPositive = true }
            };
        }

        public async Task<List<ActivityItem>> GetRecentActivitiesAsync()
        {
            await Task.Delay(400);

            return new List<ActivityItem>
            {
                new ActivityItem { Description = "Sale completed (INV-10023)", Time = "10 mins ago", Icon = "CheckCircleOutline", Color = "Green" },
                new ActivityItem { Description = "New Customer registered (John Doe)", Time = "25 mins ago", Icon = "AccountPlusOutline", Color = "Blue" },
                new ActivityItem { Description = "Purchase Order created (PO-500)", Time = "1 hour ago", Icon = "FileDocumentPlusOutline", Color = "Orange" },
                new ActivityItem { Description = "Expense recorded (Utility Bill)", Time = "2 hours ago", Icon = "CurrencyUsdOff", Color = "Red" },
                new ActivityItem { Description = "Product inventory updated (MacBook Pro)", Time = "3 hours ago", Icon = "Update", Color = "Purple" }
            };
        }

        public async Task<List<NotificationItem>> GetNotificationsAsync()
        {
            await Task.Delay(300);

            return new List<NotificationItem>
            {
                new NotificationItem { Title = "Low Stock Alert", Message = "12 products are running low on stock.", Type = "Warning" },
                new NotificationItem { Title = "Installments Due", Message = "5 installments are due today.", Type = "Info" },
                new NotificationItem { Title = "Database Backup", Message = "Remember to backup your database this week.", Type = "Info" }
            };
        }

        public async Task<List<SystemStatusItem>> GetSystemStatusAsync()
        {
            await Task.Delay(200);

            return new List<SystemStatusItem>
            {
                new SystemStatusItem { Name = "Database Status", Status = "Online", IsHealthy = true },
                new SystemStatusItem { Name = "Backup Status", Status = "Up to date", IsHealthy = true },
                new SystemStatusItem { Name = "License Status", Status = "Active (365 Days)", IsHealthy = true },
                new SystemStatusItem { Name = "Application Version", Status = "v1.0.0", IsHealthy = true },
                new SystemStatusItem { Name = "Current User", Status = "Admin", IsHealthy = true },
                new SystemStatusItem { Name = "Storage Used", Status = "450 MB / 50 GB", IsHealthy = true }
            };
        }

        public async Task<ISeries[]> GetDailySalesChartAsync()
        {
            await Task.Delay(600);
            return new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 2, 1, 3, 5, 3, 4, 6, 8, 7, 5, 4, 6, 8, 9, 7, 10, 12, 11, 13, 15, 14, 16, 18, 17, 20, 19, 22, 21, 25, 24 },
                    Fill = null,
                    GeometrySize = 0
                }
            };
        }

        public async Task<ISeries[]> GetMonthlySalesChartAsync()
        {
            await Task.Delay(500);
            return new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { 200, 500, 400, 500, 300, 400, 600, 800, 700, 500, 400, 600 }
                }
            };
        }

        public async Task<ISeries[]> GetPurchasesVsSalesChartAsync()
        {
            await Task.Delay(500);
            return new ISeries[]
            {
                new ColumnSeries<double> { Values = new double[] { 300, 400, 200, 500, 400, 600 }, Name = "Sales" },
                new ColumnSeries<double> { Values = new double[] { 150, 200, 100, 250, 200, 300 }, Name = "Purchases" }
            };
        }

        public async Task<ISeries[]> GetSalesByCategoryChartAsync()
        {
            await Task.Delay(500);
            return new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { 8 }, Name = "Electronics" },
                new PieSeries<double> { Values = new double[] { 4 }, Name = "Clothing" },
                new PieSeries<double> { Values = new double[] { 2 }, Name = "Groceries" },
                new PieSeries<double> { Values = new double[] { 1 }, Name = "Accessories" }
            };
        }
    }
}
