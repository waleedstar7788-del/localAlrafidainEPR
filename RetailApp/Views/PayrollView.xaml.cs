using System.Windows.Controls;
using RetailApp.ViewModels;

namespace RetailApp.Views
{
    public partial class PayrollView : UserControl
    {
        public PayrollView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PayrollViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
