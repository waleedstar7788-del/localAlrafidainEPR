using System.Windows.Controls;
using RetailApp.ViewModels;

namespace RetailApp.Controls
{
    public partial class SalesInvoiceEditorDialog : System.Windows.Controls.UserControl
    {
        public SalesInvoiceEditorDialog()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is SalesInvoiceEditorViewModel vm)
            {
                vm.RecalculateTotalsCommand.Execute(null);
            }
        }
    }
}
