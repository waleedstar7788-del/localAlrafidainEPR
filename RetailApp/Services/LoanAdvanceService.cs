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
    public class LoanAdvanceService : ILoanAdvanceService
    {
        private readonly AppDbContext _context;
        private readonly IJournalService _journalService;

        public LoanAdvanceService(AppDbContext context, IJournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        public async Task<List<EmployeeAdvance>> GetActiveAdvancesAsync(int employeeId)
        {
            return await _context.EmployeeAdvances
                .Where(a => a.EmployeeId == employeeId && a.Status == AdvanceStatus.Active)
                .ToListAsync();
        }

        public async Task<EmployeeAdvance> RequestAdvanceAsync(int employeeId, decimal amount, string notes)
        {
            var emp = await _context.Employees.FindAsync(employeeId);
            if (emp == null) throw new Exception("Employee not found");

            var advance = new EmployeeAdvance
            {
                EmployeeId = employeeId,
                TotalAmount = amount,
                RemainingBalance = amount,
                AdvanceNumber = "ADV-" + DateTime.Now.Ticks.ToString().Substring(10),
                Notes = notes
            };

            _context.EmployeeAdvances.Add(advance);
            await _context.SaveChangesAsync();

            // Journal Entry Integration:
            // Debit: Employee Advances Asset Account (Assume 1500)
            // Credit: Cash (1100)

            var journalLines = new List<(string AccountNumber, decimal Debit, decimal Credit)>
            {
                ("1500", amount, 0), // Debit Advances
                ("1100", 0, amount)  // Credit Cash
            };

            await _journalService.GenerateAutomaticEntryAsync(advance.AdvanceNumber, $"سلفة للموظف {emp.FullName}", journalLines);

            return advance;
        }
    }
}
