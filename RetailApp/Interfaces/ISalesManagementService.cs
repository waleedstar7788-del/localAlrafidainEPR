using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ISalesManagementService
    {
        Task<List<SalesInvoice>> GetInvoicesAsync(int pageNumber, int pageSize);
        Task<List<SalesInvoice>> SearchInvoicesAsync(string query, int pageNumber, int pageSize);
        Task<SalesInvoice?> GetInvoiceByIdAsync(int id);
        Task<SalesInvoice?> GetInvoiceWithItemsAsync(int id);
        Task CancelInvoiceAsync(int invoiceId);
        Task UpdateInvoiceAsync(SalesInvoice invoice);
        Task<List<SalesInvoice>> GetInvoicesByCustomerIdAsync(int customerId);
    }
}
