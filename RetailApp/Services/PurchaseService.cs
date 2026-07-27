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
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _context;
        private readonly IJournalService _journalService;

        public PurchaseService(AppDbContext context, IJournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        public async Task<List<PurchaseInvoice>> GetInvoicesAsync(int page, int pageSize)
        {
            return await _context.PurchaseInvoices
                .Include(p => p.Supplier)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PurchaseInvoice?> GetInvoiceByIdAsync(int id)
        {
            return await _context.PurchaseInvoices
                .Include(p => p.Supplier)
                .Include(p => p.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PurchaseInvoice?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            return await _context.PurchaseInvoices
                .Include(p => p.Supplier)
                .Include(p => p.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.InvoiceNumber == invoiceNumber);
        }

        public async Task<PurchaseInvoice> AddInvoiceAsync(PurchaseInvoice invoice)
        {
            invoice.CreatedDate = DateTime.Now;
            _context.PurchaseInvoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Generate Automatic Journal Entry
            var journalLines = new System.Collections.Generic.List<(string AccountNumber, decimal Debit, decimal Credit)>();
            journalLines.Add(("1400", invoice.TotalAmount, 0)); // Debit Inventory

            if (invoice.PaidAmount > 0)
                journalLines.Add(("1100", 0, invoice.PaidAmount)); // Credit Cash
            if (invoice.RemainingAmount > 0)
                journalLines.Add(("2100", 0, invoice.RemainingAmount)); // Credit Accounts Payable

            await _journalService.GenerateAutomaticEntryAsync(invoice.InvoiceNumber, $"فاتورة مشتريات رقم {invoice.InvoiceNumber}", journalLines);

            return invoice;
        }

        public async Task UpdateInvoiceAsync(PurchaseInvoice invoice)
        {
            invoice.ModifiedDate = DateTime.Now;
            
            // Re-calculate totals
            invoice.SubTotal = invoice.Items.Sum(i => i.SubTotal);
            invoice.TotalAmount = invoice.SubTotal - invoice.DiscountAmount + invoice.TaxAmount;

            _context.PurchaseInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.PurchaseInvoices.FindAsync(id);
            if (invoice != null)
            {
                _context.PurchaseInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CancelInvoiceAsync(int id)
        {
            var invoice = await _context.PurchaseInvoices.FindAsync(id);
            if (invoice != null && invoice.Status != InvoiceStatus.Completed)
            {
                invoice.Status = InvoiceStatus.Cancelled;
                invoice.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextInvoiceNumberAsync()
        {
            var lastInvoice = await _context.PurchaseInvoices
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (lastInvoice == null || string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                return "PINV-10001";
            }

            string numericPart = lastInvoice.InvoiceNumber.Replace("PINV-", "");
            if (int.TryParse(numericPart, out int lastNumber))
            {
                return $"PINV-{(lastNumber + 1).ToString("D5")}";
            }

            return "PINV-10001";
        }
    }
}
