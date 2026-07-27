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
    public class SalesReturnService : ISalesReturnService
    {
        private readonly AppDbContext _context;

        public SalesReturnService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesReturnInvoice>> GetReturnsAsync(int pageNumber, int pageSize)
        {
            return await _context.SalesReturnInvoices
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SalesReturnInvoice?> GetReturnByIdAsync(int id)
        {
            return await _context.SalesReturnInvoices
                .Include(r => r.Customer)
                .Include(r => r.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SalesReturnInvoice> ProcessReturnAsync(SalesReturnInvoice returnInvoice)
        {
            returnInvoice.CreatedDate = DateTime.Now;
            returnInvoice.Status = ReturnStatus.Completed;

            decimal totalRefund = 0;

            foreach (var item in returnInvoice.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    // 1. Inventory integration: Returning a sold item INCREASES stock
                    product.CurrentQuantity += item.QuantityReturned;
                    product.LastUpdated = DateTime.Now;

                    // 2. Stock Movement
                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = product.Id,
                        Date = returnInvoice.ReturnDate,
                        Time = DateTime.Now.ToString("HH:mm:ss"),
                        User = returnInvoice.CashierName,
                        Type = "Sales Return",
                        Reason = $"مرتجع مبيعات {returnInvoice.ReturnNumber}",
                        ReferenceNumber = returnInvoice.ReturnNumber,
                        QuantityChange = item.QuantityReturned,
                        ResultingQuantity = product.CurrentQuantity
                    });

                    totalRefund += item.SubTotal;
                }
            }

            returnInvoice.TotalRefundAmount = totalRefund;

            // 3. Customer Integration
            if (returnInvoice.RefundMethod == RefundMethod.Credit && returnInvoice.CustomerId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(returnInvoice.CustomerId.Value);
                if (customer != null)
                {
                    // Decreasing current balance means crediting the customer (they owe us less, or we owe them)
                    customer.CurrentBalance -= totalRefund; 
                    
                    // Decrease Total Sales because it was returned
                    customer.TotalSales -= totalRefund;
                }
            }

            // Also reduce original invoice's NetProfit/GrandTotal if needed in the future
            if (returnInvoice.SalesInvoiceId.HasValue)
            {
                var originalSale = await _context.SalesInvoices.FindAsync(returnInvoice.SalesInvoiceId.Value);
                if (originalSale != null)
                {
                    // For accounting, we could mark the invoice as Partially Refunded or reduce its recorded profit
                    // But usually creating a separate Return document is standard GAAP practice.
                }
            }

            _context.SalesReturnInvoices.Add(returnInvoice);
            await _context.SaveChangesAsync();

            return returnInvoice;
        }

        public async Task CancelReturnAsync(int returnId)
        {
            var ret = await GetReturnByIdAsync(returnId);
            if (ret != null && ret.Status != ReturnStatus.Cancelled)
            {
                ret.Status = ReturnStatus.Cancelled;

                // Revert inventory
                foreach (var item in ret.Items)
                {
                    item.Product.CurrentQuantity -= item.QuantityReturned;
                }

                // Revert customer balance
                if (ret.RefundMethod == RefundMethod.Credit && ret.Customer != null)
                {
                    ret.Customer.CurrentBalance += ret.TotalRefundAmount;
                    ret.Customer.TotalSales += ret.TotalRefundAmount;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
