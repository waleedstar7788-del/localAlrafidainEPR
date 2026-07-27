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
    public class PayrollService : IPayrollService
    {
        private readonly AppDbContext _context;
        private readonly IJournalService _journalService;

        public PayrollService(AppDbContext context, IJournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        public async Task<List<PayrollRecord>> GetPayrollsByMonthAsync(int month, int year)
        {
            return await _context.PayrollRecords
                .Include(p => p.Employee)
                .Where(p => p.Month == month && p.Year == year)
                .ToListAsync();
        }

        public async Task<List<PayrollRecord>> GenerateMonthlyPayrollAsync(int month, int year)
        {
            // 1. Check if already generated
            var existing = await GetPayrollsByMonthAsync(month, year);
            if (existing.Any()) return existing;

            // 2. Get active employees
            var employees = await _context.Employees
                .Where(e => e.Status == EmploymentStatus.Active)
                .ToListAsync();

            var payrolls = new List<PayrollRecord>();

            foreach (var emp in employees)
            {
                // Calculate simple deductions:
                // Check for active advances
                var advances = await _context.EmployeeAdvances
                    .Where(a => a.EmployeeId == emp.Id && a.Status == AdvanceStatus.Active && a.RemainingBalance > 0)
                    .ToListAsync();

                // For simplicity, deduct up to 20% of salary for advances, or full balance if less
                decimal advanceDeduction = 0;
                foreach(var adv in advances)
                {
                    decimal maxDeduction = emp.MonthlySalary * 0.20m;
                    decimal deduction = Math.Min(maxDeduction, adv.RemainingBalance);
                    advanceDeduction += deduction;
                    
                    // Note: We don't update the Advance balance here until it's actually PAID.
                }

                decimal baseSal = emp.MonthlySalary;
                decimal bonuses = emp.StandardAllowance; // simplification
                decimal net = baseSal + bonuses - advanceDeduction;

                var payroll = new PayrollRecord
                {
                    PayrollNumber = $"PAY-{year}-{month}-{emp.Id}",
                    EmployeeId = emp.Id,
                    Month = month,
                    Year = year,
                    BaseSalary = baseSal,
                    TotalAllowances = bonuses,
                    AdvanceDeduction = advanceDeduction,
                    NetSalary = net,
                    Status = PayrollStatus.Draft
                };

                payrolls.Add(payroll);
            }

            _context.PayrollRecords.AddRange(payrolls);
            await _context.SaveChangesAsync();

            return payrolls;
        }

        public async Task PaySalaryAsync(int payrollId, FinancialPaymentMethod paymentMethod)
        {
            var payroll = await _context.PayrollRecords.Include(p => p.Employee).FirstOrDefaultAsync(p => p.Id == payrollId);
            if (payroll == null || payroll.Status == PayrollStatus.Paid) throw new Exception("Invalid or already paid payroll.");

            // 1. Mark as Paid
            payroll.Status = PayrollStatus.Paid;
            payroll.PaymentDate = DateTime.Now;
            payroll.PaymentMethod = paymentMethod;

            // 2. Update Advance Balances
            if (payroll.AdvanceDeduction > 0)
            {
                var advances = await _context.EmployeeAdvances
                    .Where(a => a.EmployeeId == payroll.EmployeeId && a.Status == AdvanceStatus.Active && a.RemainingBalance > 0)
                    .ToListAsync();

                decimal amountToDeduct = payroll.AdvanceDeduction;
                foreach(var adv in advances)
                {
                    if (amountToDeduct <= 0) break;
                    decimal deduct = Math.Min(amountToDeduct, adv.RemainingBalance);
                    adv.RemainingBalance -= deduct;
                    amountToDeduct -= deduct;

                    if (adv.RemainingBalance <= 0) adv.Status = AdvanceStatus.PaidOff;
                }
            }

            await _context.SaveChangesAsync();

            // 3. Journal Entry Integration
            // Debit: Salaries Expense (5300) -> Full Gross Salary
            // Credit: Employee Advances (1500) -> advanceDeduction
            // Credit: Cash (1100) or Bank (1200) -> NetSalary

            string creditAccount = paymentMethod == FinancialPaymentMethod.Cash ? "1100" : "1200";
            decimal grossSalary = payroll.BaseSalary + payroll.TotalAllowances + payroll.TotalBonuses;

            var journalLines = new List<(string AccountNumber, decimal Debit, decimal Credit)>
            {
                ("5300", grossSalary, 0), // Debit Salaries Expense
                (creditAccount, 0, payroll.NetSalary) // Credit Cash for Net Amount
            };

            if (payroll.AdvanceDeduction > 0)
            {
                journalLines.Add(("1500", 0, payroll.AdvanceDeduction)); // Credit Advances Asset (reducing asset)
            }

            await _journalService.GenerateAutomaticEntryAsync($"SAL-{payroll.PayrollNumber}", $"دفع راتب {payroll.Month}/{payroll.Year} للموظف {payroll.Employee.FullName}", journalLines);
        }
    }
}
