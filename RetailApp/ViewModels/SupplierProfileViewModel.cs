using CommunityToolkit.Mvvm.ComponentModel;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.ViewModels
{
    public partial class SupplierProfileViewModel : BaseViewModel
    {
        private readonly ISupplierService _supplierService;
        
        [ObservableProperty]
        private Supplier _supplier = null!;

        public SupplierProfileViewModel(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public void Initialize(Supplier supplier)
        {
            Supplier = supplier;
        }
    }
}
