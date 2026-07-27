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
    public partial class IncomeEditorViewModel : BaseViewModel
    {
        private readonly IIncomeService _incomeService;
        private readonly IAccountingService _accountingService;

        public IncomeEditorViewModel(IIncomeService incomeService, IAccountingService accountingService)
        {
            _incomeService = incomeService;
            _accountingService = accountingService;
        }

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private decimal _amount;
        [ObservableProperty] private FinancialPaymentMethod _paymentMethod = FinancialPaymentMethod.Cash;
        [ObservableProperty] private string _referenceNumber = string.Empty;
        [ObservableProperty] private Account? _selectedAccount;
        
        [ObservableProperty] private ObservableCollection<Account> _revenueAccounts = new();
        [ObservableProperty] private ObservableCollection<FinancialPaymentMethod> _paymentMethods = new();

        public async Task LoadDataAsync()
        {
            var accounts = await _accountingService.GetAllAccountsAsync();
            // Assuming Revenue accounts start with 4 (e.g. 4100, 4200)
            var revenueAccs = accounts.Where(a => a.Category == AccountCategory.Revenue).ToList();
            
            RevenueAccounts.Clear();
            foreach (var acc in revenueAccs) RevenueAccounts.Add(acc);

            PaymentMethods.Clear();
            PaymentMethods.Add(FinancialPaymentMethod.Cash);
            PaymentMethods.Add(FinancialPaymentMethod.Bank);
            PaymentMethods.Add(FinancialPaymentMethod.Card);
            PaymentMethods.Add(FinancialPaymentMethod.Transfer);
        }

        [RelayCommand]
        private async Task SaveAsync(object? windowInstance)
        {
            if (Amount <= 0 || SelectedAccount == null || string.IsNullOrWhiteSpace(Title))
            {
                return;
            }

            var income = new IncomeEntry
            {
                Title = Title,
                Description = Description,
                Amount = Amount,
                PaymentMethod = PaymentMethod,
                ReferenceNumber = ReferenceNumber,
                AccountId = SelectedAccount.Id,
                IncomeDate = DateTime.Now,
                CreatedBy = "Admin"
            };

            await _incomeService.AddIncomeAsync(income);
            
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
