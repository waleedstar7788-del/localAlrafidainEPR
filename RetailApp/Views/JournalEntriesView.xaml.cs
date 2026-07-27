using System.Windows.Controls;
using RetailApp.ViewModels;

namespace RetailApp.Views
{
    public partial class JournalEntriesView : UserControl
    {
        public JournalEntriesView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is JournalEntriesViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
