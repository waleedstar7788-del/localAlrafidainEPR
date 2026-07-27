using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IReportingService
    {
        Task<decimal> GetTotalSalesAsync(DateTime start, DateTime end);
        Task<decimal> GetTotalPurchasesAsync(DateTime start, DateTime end);
        Task<decimal> GetTotalExpensesAsync(DateTime start, DateTime end);
        Task<decimal> GetTotalIncomeAsync(DateTime start, DateTime end);
        Task<decimal> GetInventoryValuationAsync();
        
        Task<Dictionary<string, decimal>> GetTopSellingProductsAsync(int count);
        Task<Dictionary<DateTime, decimal>> GetSalesTrendAsync(int days);
    }
}
