using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class SalesService : ISalesService
    {
        private readonly AppDbContext _context;
        private readonly IJournalService _journalService;

        public SalesService(AppDbContext context, IJournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        public Task<bool> ValidateSaleAsync(Invoice invoice)
        {
            if (invoice == null || !invoice.Items.Any())
            {
                return Task.FromResult(false);
            }

            foreach (var item in invoice.Items)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    // For a real app, maybe return specific error messages via an out parameter or exception
                    return Task.FromResult(false); 
                }
            }

            return Task.FromResult(true);
        }

        public async Task<Invoice> CompleteSaleAsync(Invoice invoice)
        {
            // 1. Map UI Invoice to Database SalesInvoice
            var salesInvoice = new SalesInvoice
            {
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.Date,
                CustomerId = invoice.Customer?.Id > 0 ? invoice.Customer.Id : null,
                CashierName = invoice.Cashier,
                PaymentMethod = invoice.PaymentType,
                SubTotal = invoice.SubTotal,
                DiscountAmount = invoice.Discount,
                TaxAmount = invoice.Tax,
                GrandTotal = invoice.GrandTotal,
                PaidAmount = invoice.PaidAmount,
                RemainingAmount = invoice.RemainingAmount,
                Status = invoice.RemainingAmount > 0 ? SalesInvoiceStatus.Partial : SalesInvoiceStatus.Paid,
                CreatedDate = DateTime.Now
            };

            if (salesInvoice.PaidAmount == 0 && salesInvoice.GrandTotal > 0)
            {
                salesInvoice.Status = SalesInvoiceStatus.Unpaid;
            }

            decimal totalCost = 0;

            // 2. Map Items and Apply Inventory Updates
            foreach (var cartItem in invoice.Items)
            {
                var product = await _context.Products.FindAsync(cartItem.Product.Id);
                if (product != null)
                {
                    decimal unitCost = product.CostPrice; // Snapshot cost
                    
                    var salesItem = new SalesItem
                    {
                        ProductId = product.Id,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice,
                        UnitCost = unitCost,
                        Discount = cartItem.Discount,
                        SubTotal = cartItem.Total
                    };

                    salesInvoice.Items.Add(salesItem);
                    totalCost += (unitCost * cartItem.Quantity);

                    // Reduce Stock
                    product.CurrentQuantity -= cartItem.Quantity;
                    product.LastUpdated = DateTime.Now;

                    // Log Stock Movement
                    _context.StockMovements.Add(new StockMovement
                    {
                        ProductId = product.Id,
                        Date = invoice.Date,
                        Time = DateTime.Now.ToString("HH:mm:ss"),
                        User = invoice.Cashier,
                        Type = "Sale",
                        Reason = $"مبيعات فاتورة {invoice.InvoiceNumber}",
                        ReferenceNumber = invoice.InvoiceNumber,
                        QuantityChange = -cartItem.Quantity,
                        ResultingQuantity = product.CurrentQuantity
                    });
                }
            }

            // 3. Profit Calculation
            salesInvoice.TotalCost = totalCost;
            salesInvoice.GrossProfit = salesInvoice.GrandTotal - totalCost;
            salesInvoice.NetProfit = salesInvoice.GrossProfit; // Can subtract taxes or overheads if needed

            _context.SalesInvoices.Add(salesInvoice);

            // 4. Update Customer Balance
            if (invoice.Customer != null)
            {
                var dbCustomer = await _context.Customers.FindAsync(invoice.Customer.Id);
                if (dbCustomer != null)
                {
                    dbCustomer.CurrentBalance += salesInvoice.RemainingAmount; // Increase debt
                    dbCustomer.TotalSales += salesInvoice.GrandTotal;
                    dbCustomer.LastPurchaseDate = invoice.Date;
                }
            }

            // 5. Commit Transaction
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }

            // 6. Generate Automatic Journal Entry
            var journalLines = new System.Collections.Generic.List<(string AccountNumber, decimal Debit, decimal Credit)>();
            if (salesInvoice.PaidAmount > 0)
                journalLines.Add(("1100", salesInvoice.PaidAmount, 0)); // Debit Cash
            if (salesInvoice.RemainingAmount > 0)
                journalLines.Add(("1300", salesInvoice.RemainingAmount, 0)); // Debit Accounts Receivable

            journalLines.Add(("4100", 0, salesInvoice.GrandTotal)); // Credit Sales Revenue

            await _journalService.GenerateAutomaticEntryAsync(salesInvoice.InvoiceNumber, $"فاتورة مبيعات رقم {salesInvoice.InvoiceNumber}", journalLines);

            // Note: Since invoice is a UI model and already bound to POS, we don't necessarily need to return a modified one, 
            // but we fulfill the interface contract.
            return invoice; 
        }
    }
}
