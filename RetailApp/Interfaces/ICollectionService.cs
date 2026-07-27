using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ICollectionService
    {
        Task<InstallmentPayment> ProcessPaymentAsync(int scheduleId, decimal amountPaid, InstallmentPaymentMethod method, string cashier, string notes);
    }
}
