using System.Windows.Controls;
namespace RetailApp.Views { public partial class ExpensesView : UserControl { 
        public ExpensesView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.ExpensesViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
