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
    public partial class ExpenseEditorViewModel : BaseViewModel
    {
        private readonly IExpenseService _expenseService;
        private readonly IAccountingService _accountingService;

        public ExpenseEditorViewModel(IExpenseService expenseService, IAccountingService accountingService)
        {
            _expenseService = expenseService;
            _accountingService = accountingService;
        }

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private decimal _amount;
        [ObservableProperty] private FinancialPaymentMethod _paymentMethod = FinancialPaymentMethod.Cash;
        [ObservableProperty] private string _referenceNumber = string.Empty;
        [ObservableProperty] private Account? _selectedAccount;
        
        [ObservableProperty] private ObservableCollection<Account> _expenseAccounts = new();
        [ObservableProperty] private ObservableCollection<FinancialPaymentMethod> _paymentMethods = new();

        public async Task LoadDataAsync()
        {
            var accounts = await _accountingService.GetAllAccountsAsync();
            // Assuming Expense accounts start with 5 (e.g. 5100, 5200)
            var expenseAccs = accounts.Where(a => a.Category == AccountCategory.Expense).ToList();
            
            ExpenseAccounts.Clear();
            foreach (var acc in expenseAccs) ExpenseAccounts.Add(acc);

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

            var expense = new ExpenseEntry
            {
                Title = Title,
                Description = Description,
                Amount = Amount,
                PaymentMethod = PaymentMethod,
                ReferenceNumber = ReferenceNumber,
                AccountId = SelectedAccount.Id,
                ExpenseDate = DateTime.Now,
                CreatedBy = "Admin" // TODO: Get from auth service
            };

            await _expenseService.AddExpenseAsync(expense);
            
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
