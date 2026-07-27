using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetCustomersAsync(int page, int pageSize)
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer?> GetCustomerByNumberAsync(string customerNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerNumber == customerNumber);
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            customer.CreatedDate = DateTime.Now;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            customer.ModifiedDate = DateTime.Now;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ArchiveCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.IsActive = false;
                customer.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextCustomerNumberAsync()
        {
            var lastCustomer = await _context.Customers
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (lastCustomer == null || string.IsNullOrEmpty(lastCustomer.CustomerNumber))
            {
                return "10001";
            }

            if (int.TryParse(lastCustomer.CustomerNumber, out int lastNumber))
            {
                return (lastNumber + 1).ToString("D5");
            }

            // Fallback if numbers were alphanumeric
            return "10001";
        }
    }
}
