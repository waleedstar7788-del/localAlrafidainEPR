using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class SalesManagementService : ISalesManagementService
    {
        private readonly AppDbContext _context;

        public SalesManagementService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesInvoice>> GetInvoicesAsync(int pageNumber, int pageSize)
        {
            return await _context.SalesInvoices
                .Include(s => s.Customer)
                .OrderByDescending(s => s.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<SalesInvoice>> SearchInvoicesAsync(string query, int pageNumber, int pageSize)
        {
            return await _context.SalesInvoices
                .Include(s => s.Customer)
                .Where(s => s.InvoiceNumber.Contains(query) || 
                            s.CashierName.Contains(query) || 
                            (s.Customer != null && s.Customer.FullName.Contains(query)))
                .OrderByDescending(s => s.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SalesInvoice?> GetInvoiceByIdAsync(int id)
        {
            return await _context.SalesInvoices
                .Include(s => s.Customer)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task CancelInvoiceAsync(int invoiceId)
        {
            var invoice = await GetInvoiceByIdAsync(invoiceId);
            if (invoice != null && invoice.Status != SalesInvoiceStatus.Cancelled)
            {
                invoice.Status = SalesInvoiceStatus.Cancelled;
                
                // Revert stock
                foreach(var item in invoice.Items)
                {
                    item.Product.CurrentQuantity += item.Quantity;
                }

                // Revert customer balance if unpaid
                if (invoice.Customer != null)
                {
                    invoice.Customer.CurrentBalance -= invoice.RemainingAmount;
                    invoice.Customer.TotalSales -= invoice.GrandTotal;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<SalesInvoice?> GetInvoiceWithItemsAsync(int id)
        {
            return await _context.SalesInvoices
                .Include(s => s.Customer)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateInvoiceAsync(SalesInvoice invoice)
        {
            // Recalculate totals
            invoice.SubTotal = invoice.Items.Sum(i => i.SubTotal);
            invoice.GrandTotal = invoice.SubTotal + invoice.TaxAmount - invoice.DiscountAmount;
            invoice.RemainingAmount = invoice.GrandTotal - invoice.PaidAmount;
            invoice.ModifiedDate = System.DateTime.Now;

            // Remove deleted items
            var existingItems = await _context.SalesItems
                .Where(i => i.SalesInvoiceId == invoice.Id)
                .ToListAsync();

            var currentItemIds = invoice.Items.Where(i => i.Id > 0).Select(i => i.Id).ToHashSet();
            var itemsToRemove = existingItems.Where(i => !currentItemIds.Contains(i.Id)).ToList();
            _context.SalesItems.RemoveRange(itemsToRemove);

            // Add new items (Id == 0)
            foreach (var item in invoice.Items.Where(i => i.Id == 0))
            {
                item.SalesInvoiceId = invoice.Id;
                _context.SalesItems.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<SalesInvoice>> GetInvoicesByCustomerIdAsync(int customerId)
        {
            return await _context.SalesInvoices
                .Include(s => s.Items)
                .Where(s => s.CustomerId == customerId)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }
    }
}
