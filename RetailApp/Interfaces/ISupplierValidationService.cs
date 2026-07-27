using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ISupplierValidationService
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateSupplierAsync(Supplier supplier);
    }
}
