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
    public class InstallmentService : IInstallmentService
    {
        private readonly AppDbContext _context;

        public InstallmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InstallmentContract>> GetContractsAsync(int pageNumber, int pageSize)
        {
            return await _context.InstallmentContracts
                .Include(c => c.Customer)
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<InstallmentContract?> GetContractByIdAsync(int id)
        {
            return await _context.InstallmentContracts
                .Include(c => c.Customer)
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<InstallmentContract> GenerateContractAsync(InstallmentContract contract)
        {
            contract.CreatedDate = DateTime.Now;
            contract.RemainingAmount = contract.TotalAmount; // initially
            contract.Status = ContractStatus.Active;

            // Generate mathematically precise schedules
            decimal amountPerInstallment = contract.TotalAmount / contract.NumberOfInstallments;
            DateTime currentDueDate = contract.StartDate;

            for (int i = 1; i <= contract.NumberOfInstallments; i++)
            {
                contract.Schedules.Add(new InstallmentSchedule
                {
                    InstallmentNumber = i,
                    DueDate = currentDueDate,
                    Amount = amountPerInstallment,
                    PaidAmount = 0,
                    Status = ScheduleStatus.Pending
                });

                // Increment date based on type
                if (contract.InstallmentType == InstallmentType.Monthly)
                    currentDueDate = currentDueDate.AddMonths(1);
                else if (contract.InstallmentType == InstallmentType.Weekly)
                    currentDueDate = currentDueDate.AddDays(7);
                else if (contract.InstallmentType == InstallmentType.Daily)
                    currentDueDate = currentDueDate.AddDays(1);
            }

            contract.EndDate = currentDueDate; // Set the actual end date based on final schedule

            // Adjust Customer Balance (Increasing their debt)
            var customer = await _context.Customers.FindAsync(contract.CustomerId);
            if (customer != null)
            {
                customer.CurrentBalance += contract.TotalAmount;
            }

            _context.InstallmentContracts.Add(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task CancelContractAsync(int contractId)
        {
            var contract = await GetContractByIdAsync(contractId);
            if (contract != null && contract.Status != ContractStatus.Cancelled)
            {
                // Can only cancel if no payments made, or we have to revert payments
                bool hasPayments = contract.Schedules.Any(s => s.PaidAmount > 0);
                if (hasPayments)
                {
                    throw new InvalidOperationException("Cannot cancel contract with existing payments.");
                }

                contract.Status = ContractStatus.Cancelled;

                var customer = await _context.Customers.FindAsync(contract.CustomerId);
                if (customer != null)
                {
                    customer.CurrentBalance -= contract.TotalAmount; // Revert debt
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
