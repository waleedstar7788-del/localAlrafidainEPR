using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPurchaseReturnService
    {
        Task<List<PurchaseReturnInvoice>> GetReturnsAsync(int pageNumber, int pageSize);
        Task<PurchaseReturnInvoice?> GetReturnByIdAsync(int id);
        Task<PurchaseReturnInvoice> ProcessReturnAsync(PurchaseReturnInvoice returnInvoice);
        Task CancelReturnAsync(int returnId);
    }
}
