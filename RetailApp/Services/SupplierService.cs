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
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;

        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetSuppliersAsync(int page, int pageSize)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<Supplier?> GetSupplierByNumberAsync(string supplierNumber)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierNumber == supplierNumber);
        }

        public async Task<int> GetTotalSuppliersCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        public async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            supplier.CreatedDate = DateTime.Now;
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            supplier.ModifiedDate = DateTime.Now;
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ArchiveSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                supplier.IsActive = false;
                supplier.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateNextSupplierNumberAsync()
        {
            var lastSupplier = await _context.Suppliers
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            if (lastSupplier == null || string.IsNullOrEmpty(lastSupplier.SupplierNumber))
            {
                return "SUP-10001";
            }

            // Parse numeric part e.g., "SUP-10001" -> 10001
            string numericPart = lastSupplier.SupplierNumber.Replace("SUP-", "");
            if (int.TryParse(numericPart, out int lastNumber))
            {
                return $"SUP-{(lastNumber + 1).ToString("D5")}";
            }

            return "SUP-10001";
        }
    }
}
