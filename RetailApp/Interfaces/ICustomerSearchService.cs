using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ICustomerSearchService
    {
        Task<List<Customer>> SearchCustomersAsync(string query, int page = 1, int pageSize = 50);
        Task<List<Customer>> FilterCustomersAsync(CustomerType? type, bool? isActive, string? city = null);
    }
}
