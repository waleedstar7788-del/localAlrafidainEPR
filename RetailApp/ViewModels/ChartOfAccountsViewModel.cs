using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class ChartOfAccountsViewModel : BaseViewModel
    {
        private readonly IAccountingService _accountingService;

        public ChartOfAccountsViewModel(IAccountingService accountingService)
        {
            _accountingService = accountingService;
        }

        [ObservableProperty] private ObservableCollection<Account> _accounts = new();
        [ObservableProperty] private Account? _selectedAccount;

        public async Task LoadDataAsync()
        {
            var accounts = await _accountingService.GetAllAccountsAsync();
            Accounts.Clear();
            foreach (var acc in accounts)
            {
                Accounts.Add(acc);
            }
        }
    }
}
