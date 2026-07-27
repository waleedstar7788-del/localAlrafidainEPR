using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ISupplierSearchService
    {
        Task<List<Supplier>> SearchSuppliersAsync(string query, int page = 1, int pageSize = 50);
    }
}
