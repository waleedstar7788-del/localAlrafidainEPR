using System.Windows.Controls;
using RetailApp.ViewModels;

namespace RetailApp.Views
{
    public partial class InstallmentsView : UserControl
    {
        public InstallmentsView()
        {
            InitializeComponent();
        }
        
        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is InstallmentsViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
