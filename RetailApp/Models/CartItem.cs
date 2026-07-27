using CommunityToolkit.Mvvm.ComponentModel;

namespace RetailApp.Models
{
    public partial class CartItem : ObservableObject
    {
        [ObservableProperty]
        private Product _product = new Product();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private int _quantity = 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private decimal _unitPrice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private decimal _discount;

        [ObservableProperty]
        private string _notes = string.Empty;

        public decimal Total => (UnitPrice * Quantity) - Discount;
    }
}
