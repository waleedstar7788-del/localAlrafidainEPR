using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class InstallmentPaymentViewModel : BaseViewModel
    {
        private readonly ICollectionService _collectionService;
        public InstallmentSchedule Schedule { get; }

        public InstallmentPaymentViewModel(ICollectionService collectionService, InstallmentSchedule schedule)
        {
            _collectionService = collectionService;
            Schedule = schedule;
            AmountToPay = schedule.RemainingAmount;
        }

        [ObservableProperty] private decimal _amountToPay;
        [ObservableProperty] private InstallmentPaymentMethod _paymentMethod = InstallmentPaymentMethod.Cash;
        [ObservableProperty] private string _notes = string.Empty;

        [RelayCommand]
        private async Task ProcessPaymentAsync(object? windowInstance)
        {
            if (AmountToPay <= 0 || AmountToPay > Schedule.RemainingAmount) return;

            await _collectionService.ProcessPaymentAsync(Schedule.Id, AmountToPay, PaymentMethod, "Admin", Notes);
            
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
