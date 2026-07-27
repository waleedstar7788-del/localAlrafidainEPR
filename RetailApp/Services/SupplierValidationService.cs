using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class SupplierValidationService : ISupplierValidationService
    {
        private readonly AppDbContext _context;

        public SupplierValidationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateSupplierAsync(Supplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
                return (false, "اسم المورد مطلوب.");

            if (string.IsNullOrWhiteSpace(supplier.Phone1))
                return (false, "رقم الهاتف الأساسي مطلوب.");

            if (!Regex.IsMatch(supplier.Phone1, @"^\+?[0-9]{9,15}$"))
                return (false, "رقم الهاتف الأساسي غير صالح. يجب أن يحتوي على 9 أرقام على الأقل.");

            bool phoneExists = await _context.Suppliers
                .AnyAsync(s => s.Id != supplier.Id && s.Phone1 == supplier.Phone1);

            if (phoneExists)
                return (false, "رقم الهاتف هذا مسجل مسبقاً لمورد آخر.");

            if (!string.IsNullOrWhiteSpace(supplier.Email))
            {
                if (!Regex.IsMatch(supplier.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return (false, "صيغة البريد الإلكتروني غير صحيحة.");
            }

            return (true, string.Empty);
        }
    }
}
