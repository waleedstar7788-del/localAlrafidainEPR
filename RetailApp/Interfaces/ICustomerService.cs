using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetCustomersAsync(int page, int pageSize);
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByNumberAsync(string customerNumber);
        Task<int> GetTotalCustomersCountAsync();
        
        Task<Customer> AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id); // Hard delete
        Task ArchiveCustomerAsync(int id); // Soft delete (IsActive = false)
        
        Task<string> GenerateNextCustomerNumberAsync();
    }
}
