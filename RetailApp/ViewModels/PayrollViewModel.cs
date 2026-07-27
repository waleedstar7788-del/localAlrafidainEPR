using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class PayrollViewModel : BaseViewModel
    {
        private readonly IPayrollService _payrollService;

        public PayrollViewModel(IPayrollService payrollService)
        {
            _payrollService = payrollService;
            SelectedMonth = DateTime.Now.Month;
            SelectedYear = DateTime.Now.Year;
        }

        [ObservableProperty] private int _selectedMonth;
        [ObservableProperty] private int _selectedYear;

        [ObservableProperty] private ObservableCollection<PayrollRecord> _payrolls = new();
        [ObservableProperty] private PayrollRecord? _selectedPayroll;

        public async Task LoadDataAsync()
        {
            var list = await _payrollService.GetPayrollsByMonthAsync(SelectedMonth, SelectedYear);
            Payrolls.Clear();
            foreach (var p in list) Payrolls.Add(p);
        }

        [RelayCommand]
        private async Task GeneratePayroll()
        {
            await _payrollService.GenerateMonthlyPayrollAsync(SelectedMonth, SelectedYear);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task PaySelectedSalary()
        {
            if (SelectedPayroll != null && SelectedPayroll.Status != PayrollStatus.Paid)
            {
                // Simple cash payment for now
                await _payrollService.PaySalaryAsync(SelectedPayroll.Id, FinancialPaymentMethod.Cash);
                await LoadDataAsync();
            }
        }
    }
}
