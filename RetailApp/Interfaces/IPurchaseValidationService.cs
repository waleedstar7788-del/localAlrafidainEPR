using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPurchaseValidationService
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateInvoiceAsync(PurchaseInvoice invoice);
    }
}
