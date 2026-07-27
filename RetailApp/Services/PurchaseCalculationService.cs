using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class PurchaseCalculationService : IPurchaseCalculationService
    {
        private readonly AppDbContext _context;

        public PurchaseCalculationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ProcessCompletedPurchaseAsync(PurchaseInvoice invoice)
        {
            // 1. Update Supplier Balance
            var supplier = await _context.Suppliers.FindAsync(invoice.SupplierId);
            if (supplier != null)
            {
                supplier.TotalPurchases += invoice.TotalAmount;
                supplier.TotalPayments += invoice.PaidAmount;
                supplier.CurrentBalance += invoice.RemainingAmount; // Add to debt
                supplier.LastPurchaseDate = invoice.InvoiceDate;
                if (invoice.PaidAmount > 0)
                {
                    supplier.LastPaymentDate = invoice.InvoiceDate;
                }
            }

            // 2. Update Inventory and Cost for each Item
            foreach (var item in invoice.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    int oldQty = product.CurrentQuantity;
                    decimal oldCost = product.CostPrice;

                    // Calculate Moving Average Cost
                    // ((OldQty * OldCost) + (NewQty * NewCost)) / (OldQty + NewQty)
                    int newQty = oldQty + item.Quantity;
                    if (newQty > 0)
                    {
                        product.CostPrice = ((oldQty * oldCost) + (item.Quantity * item.PurchasePrice)) / newQty;
                    }
                    
                    product.PurchasePrice = item.PurchasePrice; // Update Last Purchase Price
                    product.CurrentQuantity = newQty;
                    product.LastUpdated = DateTime.Now;

                    // 3. Create Stock Movement
                    var movement = new StockMovement
                    {
                        ProductId = product.Id,
                        Date = invoice.InvoiceDate,
                        Time = DateTime.Now.ToString("HH:mm:ss"),
                        User = invoice.EmployeeName,
                        Type = "Purchase",
                        Reason = "شراء بضاعة جديدة",
                        ReferenceNumber = invoice.InvoiceNumber,
                        QuantityChange = item.Quantity,
                        ResultingQuantity = product.CurrentQuantity
                    };
                    _context.StockMovements.Add(movement);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task ReversePurchaseAsync(PurchaseInvoice invoice)
        {
            // Logic to reverse the purchase (subtract stock, deduct supplier balance)
            // if an invoice is cancelled after being completed.
            var supplier = await _context.Suppliers.FindAsync(invoice.SupplierId);
            if (supplier != null)
            {
                supplier.TotalPurchases -= invoice.TotalAmount;
                supplier.TotalPayments -= invoice.PaidAmount;
                supplier.CurrentBalance -= invoice.RemainingAmount;
            }

            foreach (var item in invoice.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.CurrentQuantity -= item.Quantity;
                    product.LastUpdated = DateTime.Now;

                    var movement = new StockMovement
                    {
                        ProductId = product.Id,
                        Date = DateTime.Now,
                        Time = DateTime.Now.ToString("HH:mm:ss"),
                        User = invoice.EmployeeName,
                        Type = "Cancel Purchase",
                        Reason = "إلغاء فاتورة مشتريات",
                        ReferenceNumber = invoice.InvoiceNumber,
                        QuantityChange = -item.Quantity,
                        ResultingQuantity = product.CurrentQuantity
                    };
                    _context.StockMovements.Add(movement);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
