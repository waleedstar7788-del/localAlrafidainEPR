using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ISalesReturnService
    {
        Task<List<SalesReturnInvoice>> GetReturnsAsync(int pageNumber, int pageSize);
        Task<SalesReturnInvoice?> GetReturnByIdAsync(int id);
        Task<SalesReturnInvoice> ProcessReturnAsync(SalesReturnInvoice returnInvoice);
        Task CancelReturnAsync(int returnId);
    }
}
