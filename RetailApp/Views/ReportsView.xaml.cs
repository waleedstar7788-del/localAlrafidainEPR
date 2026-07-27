using System.Windows.Controls;
using RetailApp.ViewModels;

namespace RetailApp.Views
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ReportsViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
