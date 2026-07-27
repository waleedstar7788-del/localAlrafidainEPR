using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class ExpensesViewModel : BaseViewModel
    {
        private readonly IExpenseService _expenseService;
        private readonly IIncomeService _incomeService;
        private readonly IExpenseStatisticsService _statisticsService;
        private readonly IDialogService _dialogService;

        public ExpensesViewModel(
            IExpenseService expenseService,
            IIncomeService incomeService,
            IExpenseStatisticsService statisticsService,
            IDialogService dialogService)
        {
            _expenseService = expenseService;
            _incomeService = incomeService;
            _statisticsService = statisticsService;
            _dialogService = dialogService;
        }

        // Dashboard Stats
        [ObservableProperty] private decimal _todaysExpenses;
        [ObservableProperty] private decimal _monthlyExpenses;
        [ObservableProperty] private decimal _todaysIncome;
        [ObservableProperty] private decimal _monthlyIncome;
        [ObservableProperty] private decimal _netIncome;

        // Lists
        [ObservableProperty] private ObservableCollection<ExpenseEntry> _expenses = new();
        [ObservableProperty] private ObservableCollection<IncomeEntry> _incomes = new();

        public async Task LoadDataAsync()
        {
            await LoadStatisticsAsync();
            await LoadListsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            TodaysExpenses = await _statisticsService.GetTodaysExpensesAsync();
            MonthlyExpenses = await _statisticsService.GetMonthlyExpensesAsync();
            TodaysIncome = await _statisticsService.GetTodaysIncomeAsync();
            MonthlyIncome = await _statisticsService.GetMonthlyIncomeAsync();
            NetIncome = MonthlyIncome - MonthlyExpenses;
        }

        private async Task LoadListsAsync()
        {
            var expList = await _expenseService.GetExpensesAsync();
            Expenses.Clear();
            foreach (var e in expList) Expenses.Add(e);

            var incList = await _incomeService.GetIncomesAsync();
            Incomes.Clear();
            foreach (var i in incList) Incomes.Add(i);
        }

        [RelayCommand]
        private async Task ShowNewExpenseDialog()
        {
            var result = await _dialogService.ShowDialogAsync("ExpenseEditorDialog", null);
            if (result)
            {
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task ShowNewIncomeDialog()
        {
            var result = await _dialogService.ShowDialogAsync("IncomeEditorDialog", null);
            if (result)
            {
                await LoadDataAsync();
            }
        }
    }
}
