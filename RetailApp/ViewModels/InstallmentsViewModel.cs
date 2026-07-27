using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class InstallmentsViewModel : BaseViewModel
    {
        private readonly IInstallmentService _installmentService;
        private readonly IInstallmentStatisticsService _statisticsService;
        private readonly IDialogService _dialogService;

        public InstallmentsViewModel(
            IInstallmentService installmentService,
            IInstallmentStatisticsService statisticsService,
            IDialogService dialogService)
        {
            _installmentService = installmentService;
            _statisticsService = statisticsService;
            _dialogService = dialogService;
        }

        [ObservableProperty] private decimal _totalOutstandingDebt;
        [ObservableProperty] private decimal _todaysCollections;
        [ObservableProperty] private decimal _monthlyCollections;
        [ObservableProperty] private int _lateInstallmentsCount;

        [ObservableProperty] private ObservableCollection<InstallmentContract> _contracts = new();
        [ObservableProperty] private InstallmentContract? _selectedContract;

        public async Task LoadDataAsync()
        {
            TotalOutstandingDebt = await _statisticsService.GetTotalOutstandingDebtAsync();
            TodaysCollections = await _statisticsService.GetTodaysCollectionsAsync();
            MonthlyCollections = await _statisticsService.GetMonthlyCollectionsAsync();
            LateInstallmentsCount = await _statisticsService.GetLateInstallmentsCountAsync();

            var list = await _installmentService.GetContractsAsync(1, 100);
            Contracts.Clear();
            foreach (var item in list)
            {
                Contracts.Add(item);
            }
        }

        [RelayCommand]
        private async Task ShowNewContractDialog()
        {
            var vm = new InstallmentContractEditorViewModel(_installmentService);
            var result = await _dialogService.ShowDialogAsync("InstallmentContractDialog", vm);
            if (result)
            {
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task ShowPaymentDialog(InstallmentSchedule? schedule)
        {
            if (schedule == null) return;
            // TODO: In a real app we'd resolve CollectionService from DI, but for simplicity here we assume the editor gets it
            // We'll pass the schedule ID to the dialog viewmodel
        }

        [RelayCommand]
        private void ViewDetails(InstallmentContract? contract)
        {
            if (contract == null) return;
            var notificationService = App.ServiceProvider.GetService(typeof(INotificationService)) as INotificationService;
            notificationService?.ShowInfo($"عرض تفاصيل العقد رقم {contract.ContractNumber}");
        }
    }
}
