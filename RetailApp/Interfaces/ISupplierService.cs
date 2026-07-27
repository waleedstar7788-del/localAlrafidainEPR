using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ISupplierService
    {
        Task<List<Supplier>> GetSuppliersAsync(int page, int pageSize);
        Task<Supplier?> GetSupplierByIdAsync(int id);
        Task<Supplier?> GetSupplierByNumberAsync(string supplierNumber);
        Task<int> GetTotalSuppliersCountAsync();
        
        Task<Supplier> AddSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(int id); // Hard delete
        Task ArchiveSupplierAsync(int id); // Soft delete (IsActive = false)
        
        Task<string> GenerateNextSupplierNumberAsync();
    }
}
