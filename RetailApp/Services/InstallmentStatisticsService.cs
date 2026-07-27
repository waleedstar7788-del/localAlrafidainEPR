using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class InstallmentStatisticsService : IInstallmentStatisticsService
    {
        private readonly AppDbContext _context;

        public InstallmentStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalOutstandingDebtAsync()
        {
            return await _context.InstallmentContracts
                .Where(c => c.Status == ContractStatus.Active)
                .SumAsync(c => c.RemainingAmount);
        }

        public async Task<decimal> GetTodaysCollectionsAsync()
        {
            var today = DateTime.Today;
            return await _context.InstallmentPayments
                .Where(p => p.PaymentDate.Date == today)
                .SumAsync(p => p.AmountPaid);
        }

        public async Task<decimal> GetMonthlyCollectionsAsync()
        {
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            return await _context.InstallmentPayments
                .Where(p => p.PaymentDate >= startOfMonth)
                .SumAsync(p => p.AmountPaid);
        }

        public async Task<int> GetLateInstallmentsCountAsync()
        {
            var today = DateTime.Today;
            return await _context.InstallmentSchedules
                .CountAsync(s => s.DueDate.Date < today && s.Status != ScheduleStatus.Paid);
        }
    }
}
