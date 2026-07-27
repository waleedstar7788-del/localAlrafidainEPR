using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface ILoanAdvanceService
    {
        Task<EmployeeAdvance> RequestAdvanceAsync(int employeeId, decimal amount, string notes);
        Task<List<EmployeeAdvance>> GetActiveAdvancesAsync(int employeeId);
    }
}
