using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace RetailApp.ViewModels
{
    public partial class InstallmentContractEditorViewModel : BaseViewModel
    {
        private readonly IInstallmentService _installmentService;

        public InstallmentContractEditorViewModel(IInstallmentService installmentService)
        {
            _installmentService = installmentService;
        }

        [ObservableProperty] private int _customerId;
        [ObservableProperty] private decimal _totalAmount;
        [ObservableProperty] private decimal _downPayment;
        [ObservableProperty] private int _numberOfInstallments = 12;
        [ObservableProperty] private InstallmentType _installmentType = InstallmentType.Monthly;

        [ObservableProperty] private ObservableCollection<InstallmentSchedule> _previewSchedules = new();

        [RelayCommand]
        private void PreviewSchedule()
        {
            PreviewSchedules.Clear();
            decimal remaining = TotalAmount - DownPayment;
            if (remaining <= 0 || NumberOfInstallments <= 0) return;

            decimal amountPer = remaining / NumberOfInstallments;
            DateTime current = DateTime.Now.AddMonths(1);

            for (int i = 1; i <= NumberOfInstallments; i++)
            {
                PreviewSchedules.Add(new InstallmentSchedule
                {
                    InstallmentNumber = i,
                    DueDate = current,
                    Amount = amountPer,
                    Status = ScheduleStatus.Pending
                });
                if (InstallmentType == InstallmentType.Monthly) current = current.AddMonths(1);
                else if (InstallmentType == InstallmentType.Weekly) current = current.AddDays(7);
            }
        }

        [RelayCommand]
        private async Task SaveAsync(object? windowInstance)
        {
            if (CustomerId <= 0 || TotalAmount <= 0) return;

            var contract = new InstallmentContract
            {
                ContractNumber = "INST-" + DateTime.Now.Ticks.ToString().Substring(10),
                CustomerId = CustomerId,
                TotalAmount = TotalAmount - DownPayment,
                DownPayment = DownPayment,
                NumberOfInstallments = NumberOfInstallments,
                InstallmentType = InstallmentType,
                StartDate = DateTime.Now.AddMonths(1)
            };

            await _installmentService.GenerateContractAsync(contract);
            
            if (windowInstance is System.Windows.Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        [RelayCommand]
        private void Cancel(object? windowInstance)
        {
            if (windowInstance is System.Windows.Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}
