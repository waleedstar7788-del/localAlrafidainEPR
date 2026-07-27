using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class CustomerValidationService : ICustomerValidationService
    {
        private readonly AppDbContext _context;

        public CustomerValidationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateCustomerAsync(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FullName))
                return (false, "اسم العميل مطلوب.");

            if (string.IsNullOrWhiteSpace(customer.Phone1))
                return (false, "رقم الهاتف الأساسي مطلوب.");

            // Basic phone format validation (minimum 9 digits)
            if (!Regex.IsMatch(customer.Phone1, @"^\+?[0-9]{9,15}$"))
                return (false, "رقم الهاتف الأساسي غير صالح. يجب أن يحتوي على 9 أرقام على الأقل.");

            // Check duplicate phone
            bool phoneExists = await _context.Customers
                .AnyAsync(c => c.Id != customer.Id && c.Phone1 == customer.Phone1);

            if (phoneExists)
                return (false, "رقم الهاتف هذا مسجل مسبقاً لعميل آخر.");

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                if (!Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return (false, "صيغة البريد الإلكتروني غير صحيحة.");
            }

            return (true, string.Empty);
        }
    }
}
