using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using RetailApp.ViewModels;

namespace RetailApp.Controls
{
    public partial class IncomeEditorDialog : UserControl
    {
        public IncomeEditorDialog()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IncomeEditorViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
