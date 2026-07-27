using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly AppDbContext _context;
        private readonly IJournalService _journalService;

        public CollectionService(AppDbContext context, IJournalService journalService)
        {
            _context = context;
            _journalService = journalService;
        }

        public async Task<InstallmentPayment> ProcessPaymentAsync(int scheduleId, decimal amountPaid, InstallmentPaymentMethod method, string cashier, string notes)
        {
            if (amountPaid <= 0)
                throw new InvalidOperationException("Payment amount must be greater than zero.");

            var schedule = await _context.InstallmentSchedules
                .Include(s => s.InstallmentContract)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
                throw new InvalidOperationException("Schedule not found.");

            if (amountPaid > schedule.RemainingAmount)
                throw new InvalidOperationException("Payment amount exceeds remaining installment amount.");

            // Create Payment Receipt
            var payment = new InstallmentPayment
            {
                InstallmentScheduleId = scheduleId,
                ReceiptNumber = "REC-" + DateTime.Now.Ticks.ToString().Substring(10),
                PaymentDate = DateTime.Now,
                AmountPaid = amountPaid,
                PaymentMethod = method,
                CashierName = cashier,
                Notes = notes
            };

            _context.InstallmentPayments.Add(payment);
            await _context.SaveChangesAsync();

            // Generate Automatic Journal Entry
            var journalLines = new System.Collections.Generic.List<(string AccountNumber, decimal Debit, decimal Credit)>
            {
                ("1100", amountPaid, 0), // Debit Cash
                ("1300", 0, amountPaid)  // Credit Accounts Receivable
            };

            await _journalService.GenerateAutomaticEntryAsync(payment.ReceiptNumber, $"سداد قسط رقم {schedule.InstallmentNumber} للعميل", journalLines);

            // Update Schedule
            schedule.PaidAmount += amountPaid;
            schedule.LastPaymentDate = payment.PaymentDate;
            schedule.Status = schedule.RemainingAmount == 0 ? ScheduleStatus.Paid : ScheduleStatus.Partial;

            // Update Contract
            var contract = schedule.InstallmentContract;
            contract.RemainingAmount -= amountPaid;
            if (contract.RemainingAmount <= 0)
            {
                contract.Status = ContractStatus.Completed;
            }

            // Update Customer Balance (reduce debt)
            var customer = await _context.Customers.FindAsync(contract.CustomerId);
            if (customer != null)
            {
                customer.CurrentBalance -= amountPaid;
                customer.TotalPaid += amountPaid;
                customer.LastPaymentDate = payment.PaymentDate;
            }

            await _context.SaveChangesAsync();

            return payment;
        }
    }
}
