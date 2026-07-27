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
    public class PurchaseReturnService : IPurchaseReturnService
    {
        private readonly AppDbContext _context;

        public PurchaseReturnService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseReturnInvoice>> GetReturnsAsync(int pageNumber, int pageSize)
        {
            return await _context.PurchaseReturnInvoices
                .Include(p => p.Supplier)
                .OrderByDescending(p => p.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PurchaseReturnInvoice?> GetReturnByIdAsync(int id)
        {
            return await _context.PurchaseReturnInvoices
                .Include(p => p.Supplier)
                .Include(p => p.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PurchaseReturnInvoice> ProcessReturnAsync(PurchaseReturnInvoice returnInvoice)
        {
            returnInvoice.CreatedDate = DateTime.Now;
            returnInvoice.Status = ReturnStatus.Completed;

            decimal totalRefund = 0;

            foreach (var item in returnInvoice.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    // 1. Inventory integration: Returning a purchased item DECREASES stock
                    product.CurrentQuantity -= item.QuantityReturned;
                    product.LastUpdated = DateTime.Now;

                    // 2. Stock Movement
                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = product.Id,
                        Date = returnInvoice.ReturnDate,
                        Time = DateTime.Now.ToString("HH:mm:ss"),
                        User = returnInvoice.EmployeeName,
                        Type = "Purchase Return",
                        Reason = $"مرتجع مشتريات {returnInvoice.ReturnNumber}",
                        ReferenceNumber = returnInvoice.ReturnNumber,
                        QuantityChange = -item.QuantityReturned,
                        ResultingQuantity = product.CurrentQuantity
                    });

                    totalRefund += item.SubTotal;
                }
            }

            returnInvoice.TotalRefundAmount = totalRefund;

            // 3. Supplier Integration
            var supplier = await _context.Suppliers.FindAsync(returnInvoice.SupplierId);
            if (supplier != null && returnInvoice.RefundMethod == RefundMethod.Credit)
            {
                // Decreasing current balance means we owe the supplier less money
                supplier.CurrentBalance -= totalRefund;
                supplier.TotalPurchases -= totalRefund;
            }

            _context.PurchaseReturnInvoices.Add(returnInvoice);
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
                    item.Product.CurrentQuantity += item.QuantityReturned;
                }

                // Revert supplier balance
                if (ret.RefundMethod == RefundMethod.Credit && ret.Supplier != null)
                {
                    ret.Supplier.CurrentBalance += ret.TotalRefundAmount;
                    ret.Supplier.TotalPurchases += ret.TotalRefundAmount;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
