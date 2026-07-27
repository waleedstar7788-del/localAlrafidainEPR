using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class JournalEditorViewModel : BaseViewModel
    {
        private readonly IJournalService _journalService;
        private readonly IAccountingService _accountingService;

        public JournalEditorViewModel(IJournalService journalService, IAccountingService accountingService)
        {
            _journalService = journalService;
            _accountingService = accountingService;
        }

        [ObservableProperty] private string _referenceNumber = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        
        [ObservableProperty] private ObservableCollection<JournalEntryLine> _lines = new();
        [ObservableProperty] private ObservableCollection<Account> _availableAccounts = new();

        [ObservableProperty] private decimal _totalDebits;
        [ObservableProperty] private decimal _totalCredits;

        public async Task LoadDataAsync()
        {
            var accounts = await _accountingService.GetAllAccountsAsync();
            AvailableAccounts.Clear();
            foreach (var acc in accounts) AvailableAccounts.Add(acc);
        }

        [RelayCommand]
        private void AddLine()
        {
            Lines.Add(new JournalEntryLine());
        }

        [RelayCommand]
        private void RemoveLine(JournalEntryLine line)
        {
            if (line != null)
            {
                Lines.Remove(line);
                UpdateTotals();
            }
        }

        public void UpdateTotals()
        {
            TotalDebits = Lines.Sum(l => l.DebitAmount);
            TotalCredits = Lines.Sum(l => l.CreditAmount);
        }

        [RelayCommand]
        private async Task SaveAsync(object? windowInstance)
        {
            UpdateTotals();
            if (TotalDebits != TotalCredits || TotalDebits == 0) return;

            var journal = new JournalEntry
            {
                ReferenceNumber = ReferenceNumber,
                Description = Description,
                CreatedBy = "Admin",
                Lines = Lines.ToList()
            };

            await _journalService.CreateJournalAsync(journal);
            
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
