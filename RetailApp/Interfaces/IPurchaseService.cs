using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPurchaseService
    {
        Task<List<PurchaseInvoice>> GetInvoicesAsync(int page, int pageSize);
        Task<PurchaseInvoice?> GetInvoiceByIdAsync(int id);
        Task<PurchaseInvoice?> GetInvoiceByNumberAsync(string invoiceNumber);
        
        Task<PurchaseInvoice> AddInvoiceAsync(PurchaseInvoice invoice);
        Task UpdateInvoiceAsync(PurchaseInvoice invoice);
        Task DeleteInvoiceAsync(int id);
        Task CancelInvoiceAsync(int id);
        
        Task<string> GenerateNextInvoiceNumberAsync();
    }
}
