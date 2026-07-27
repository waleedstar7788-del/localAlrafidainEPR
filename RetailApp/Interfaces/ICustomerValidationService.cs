using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ICustomerValidationService
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateCustomerAsync(Customer customer);
    }
}
