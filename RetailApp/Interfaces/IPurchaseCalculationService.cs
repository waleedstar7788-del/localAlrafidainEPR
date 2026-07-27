using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPurchaseCalculationService
    {
        Task ProcessCompletedPurchaseAsync(PurchaseInvoice invoice);
        Task ReversePurchaseAsync(PurchaseInvoice invoice);
    }
}
