using System.Windows.Controls;

namespace RetailApp.Views
{
    public partial class LoginView : UserControl
    {
        private bool _isUpdating = false;

        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_isUpdating) return;
            
            _isUpdating = true;
            TxtPassword.Text = TxtPasswordBox.Password;
            if (this.DataContext is ViewModels.LoginViewModel vm)
            {
                vm.Password = TxtPasswordBox.Password;
            }
            _isUpdating = false;
        }

        private void TxtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;
            
            _isUpdating = true;
            TxtPasswordBox.Password = TxtPassword.Text;
            if (this.DataContext is ViewModels.LoginViewModel vm)
            {
                vm.Password = TxtPassword.Text;
            }
            _isUpdating = false;
        }

        private void TogglePasswordVisibility_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (TxtPassword.Visibility == System.Windows.Visibility.Collapsed)
            {
                // Show clear text
                TxtPassword.Visibility = System.Windows.Visibility.Visible;
                TxtPasswordBox.Visibility = System.Windows.Visibility.Collapsed;
                IconEye.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOffOutline;
            }
            else
            {
                // Hide clear text
                TxtPassword.Visibility = System.Windows.Visibility.Collapsed;
                TxtPasswordBox.Visibility = System.Windows.Visibility.Visible;
                IconEye.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOutline;
            }
        }
    }
}
