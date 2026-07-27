using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class EmployeeEditorViewModel : BaseViewModel
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeEditorViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [ObservableProperty] private string _fullName = string.Empty;
        [ObservableProperty] private string _nationalId = string.Empty;
        [ObservableProperty] private string _phone = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _address = string.Empty;
        [ObservableProperty] private string _department = string.Empty;
        [ObservableProperty] private string _jobTitle = string.Empty;
        [ObservableProperty] private decimal _monthlySalary;

        [RelayCommand]
        private async Task SaveAsync(object? windowInstance)
        {
            if (string.IsNullOrWhiteSpace(FullName) || MonthlySalary <= 0) return;

            var emp = new Employee
            {
                FullName = FullName,
                NationalId = NationalId,
                Phone = Phone,
                Email = Email,
                Address = Address,
                Department = Department,
                JobTitle = JobTitle,
                MonthlySalary = MonthlySalary,
                HireDate = DateTime.Now
            };

            await _employeeService.AddEmployeeAsync(emp);
            
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
