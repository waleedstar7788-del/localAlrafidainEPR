using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class EmployeesViewModel : BaseViewModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDialogService _dialogService;

        public EmployeesViewModel(IEmployeeService employeeService, IDialogService dialogService)
        {
            _employeeService = employeeService;
            _dialogService = dialogService;
        }

        [ObservableProperty] private ObservableCollection<Employee> _employees = new();
        [ObservableProperty] private Employee? _selectedEmployee;

        [ObservableProperty] private int _totalEmployees;

        public async Task LoadDataAsync()
        {
            var emps = await _employeeService.GetAllEmployeesAsync();
            Employees.Clear();
            foreach (var e in emps) Employees.Add(e);

            TotalEmployees = Employees.Count;
        }

        [RelayCommand]
        private async Task ShowAddEmployeeDialog()
        {
            var result = await _dialogService.ShowDialogAsync("EmployeeEditorDialog", null);
            if (result)
            {
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task ShowEditEmployeeDialog(Employee employee)
        {
            if (employee == null) return;
            // The EmployeeEditorViewModel would load the employee. For now pass the ID or object via dialogService if supported,
            // or just show the dialog if state is handled differently.
            var result = await _dialogService.ShowDialogAsync("EmployeeEditorDialog", employee);
            if (result)
            {
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private void ViewDetails(Employee employee)
        {
            if (employee == null) return;
            var notificationService = App.ServiceProvider.GetService(typeof(INotificationService)) as INotificationService;
            notificationService?.ShowInfo($"عرض تفاصيل الموظف {employee.FullName}");
        }
    }
}
