using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class PurchaseValidationService : IPurchaseValidationService
    {
        private readonly AppDbContext _context;

        public PurchaseValidationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateInvoiceAsync(PurchaseInvoice invoice)
        {
            if (invoice.SupplierId <= 0)
                return (false, "الرجاء اختيار المورد.");

            if (invoice.Items == null || !invoice.Items.Any())
                return (false, "لا يمكن حفظ فاتورة فارغة. الرجاء إضافة منتجات.");

            foreach (var item in invoice.Items)
            {
                if (item.ProductId <= 0)
                    return (false, "يوجد منتج غير محدد في القائمة.");

                if (item.Quantity <= 0)
                    return (false, $"الكمية للمنتج '{item.Product?.Name}' يجب أن تكون أكبر من صفر.");

                if (item.PurchasePrice < 0)
                    return (false, $"سعر الشراء للمنتج '{item.Product?.Name}' غير صالح.");
            }

            return (true, string.Empty);
        }
    }
}
