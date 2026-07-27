using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using RetailApp.ViewModels;

namespace RetailApp.Controls
{
    public partial class JournalEditorDialog : UserControl
    {
        public JournalEditorDialog()
        {
            InitializeComponent();
            this.Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is JournalEditorViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (DataContext is JournalEditorViewModel vm)
            {
                // Delay to allow binding to update source
                System.Threading.Tasks.Task.Delay(100).ContinueWith(t =>
                {
                    Dispatcher.Invoke(() => vm.UpdateTotals());
                });
            }
        }
    }
}
