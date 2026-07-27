using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPayrollService
    {
        Task<List<PayrollRecord>> GenerateMonthlyPayrollAsync(int month, int year);
        Task<List<PayrollRecord>> GetPayrollsByMonthAsync(int month, int year);
        Task PaySalaryAsync(int payrollId, FinancialPaymentMethod paymentMethod);
    }
}
