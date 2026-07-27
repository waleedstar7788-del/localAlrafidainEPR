using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class CustomerEditorViewModel : BaseViewModel
    {
        private readonly ICustomerService _customerService;
        public Customer EditingCustomer { get; private set; }
        public bool IsNew { get; private set; }

        public CustomerEditorViewModel(ICustomerService customerService, Customer? customerToEdit = null)
        {
            _customerService = customerService;
            if (customerToEdit == null)
            {
                IsNew = true;
                EditingCustomer = new Customer();
                InitializeNewCustomerAsync().ConfigureAwait(false);
            }
            else
            {
                IsNew = false;
                // Clone to prevent direct editing before save
                EditingCustomer = new Customer
                {
                    Id = customerToEdit.Id,
                    CustomerNumber = customerToEdit.CustomerNumber,
                    FullName = customerToEdit.FullName,
                    CompanyName = customerToEdit.CompanyName,
                    Phone1 = customerToEdit.Phone1,
                    Email = customerToEdit.Email,
                    Address = customerToEdit.Address,
                    CreditLimit = customerToEdit.CreditLimit,
                    Rank = customerToEdit.Rank,
                    Type = customerToEdit.Type
                };
            }
        }

        private async Task InitializeNewCustomerAsync()
        {
            EditingCustomer.CustomerNumber = await _customerService.GenerateNextCustomerNumberAsync();
            OnPropertyChanged(nameof(EditingCustomer));
        }

        [RelayCommand]
        private async Task SaveAsync(object? windowInstance)
        {
            // Usually we'd validate using ICustomerValidationService here
            if (IsNew)
            {
                await _customerService.AddCustomerAsync(EditingCustomer);
            }
            else
            {
                await _customerService.UpdateCustomerAsync(EditingCustomer);
            }

            // The dialog closes and returns true
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
