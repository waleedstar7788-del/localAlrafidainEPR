using System.Windows.Controls;
using RetailApp.ViewModels;

namespace RetailApp.Views
{
    public partial class FirstRunWizardView : UserControl
    {
        public FirstRunWizardView()
        {
            InitializeComponent();
            this.Loaded += (s, e) => 
            {
                PwdBox.PasswordChanged += PwdBox_PasswordChanged;
                PwdBoxConfirm.PasswordChanged += PwdBoxConfirm_PasswordChanged;
            };
        }

        private void PwdBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is FirstRunWizardViewModel vm)
            {
                vm.AdminPassword = PwdBox.Password;
            }
        }

        private void PwdBoxConfirm_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is FirstRunWizardViewModel vm)
            {
                vm.AdminPasswordConfirm = PwdBoxConfirm.Password;
            }
        }
    }
}
