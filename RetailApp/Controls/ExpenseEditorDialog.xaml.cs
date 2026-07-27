using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using RetailApp.ViewModels;

namespace RetailApp.Controls
{
    public partial class ExpenseEditorDialog : UserControl
    {
        public ExpenseEditorDialog()
        {
            InitializeComponent();
            this.Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ExpenseEditorViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
