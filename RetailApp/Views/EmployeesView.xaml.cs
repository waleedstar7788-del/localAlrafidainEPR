using System.Windows.Controls;
using RetailApp.ViewModels;

namespace RetailApp.Views
{
    public partial class EmployeesView : UserControl
    {
        public EmployeesView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is EmployeesViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
