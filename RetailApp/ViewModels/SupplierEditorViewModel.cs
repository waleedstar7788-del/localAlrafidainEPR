using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class SupplierEditorViewModel : BaseViewModel
    {
        private readonly ISupplierService _supplierService;
        public Supplier EditingSupplier { get; private set; }
        public bool IsNew { get; private set; }

        public SupplierEditorViewModel(ISupplierService supplierService, Supplier? supplierToEdit = null)
        {
            _supplierService = supplierService;
            if (supplierToEdit == null)
            {
                IsNew = true;
                EditingSupplier = new Supplier();
                InitializeNewSupplierAsync().ConfigureAwait(false);
            }
            else
            {
                IsNew = false;
                // Clone to prevent direct editing before save
                EditingSupplier = new Supplier
                {
                    Id = supplierToEdit.Id,
                    SupplierNumber = supplierToEdit.SupplierNumber,
                    SupplierName = supplierToEdit.SupplierName,
                    CompanyName = supplierToEdit.CompanyName,
                    Phone1 = supplierToEdit.Phone1,
                    Email = supplierToEdit.Email,
                    Address = supplierToEdit.Address,
                    Type = supplierToEdit.Type,
                    TaxNumber = supplierToEdit.TaxNumber,
                    BankName = supplierToEdit.BankName,
                    IBAN = supplierToEdit.IBAN
                };
            }
        }

        private async Task InitializeNewSupplierAsync()
        {
            EditingSupplier.SupplierNumber = await _supplierService.GenerateNextSupplierNumberAsync();
            OnPropertyChanged(nameof(EditingSupplier));
        }

        [RelayCommand]
        private async Task SaveAsync(object? windowInstance)
        {
            if (IsNew)
            {
                await _supplierService.AddSupplierAsync(EditingSupplier);
            }
            else
            {
                await _supplierService.UpdateSupplierAsync(EditingSupplier);
            }

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
