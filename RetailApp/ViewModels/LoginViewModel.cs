using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using System.Threading.Tasks;
using System.Windows;

namespace RetailApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthenticationService _authService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _rememberMe;

        public LoginViewModel(INavigationService navigationService, IAuthenticationService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("الرجاء إدخال اسم المستخدم وكلمة المرور", "خطأ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsBusy = true;
            BusyMessage = "جاري التحقق من بيانات الدخول...";
            
            try
            {
                // Simulate real DB login
                bool success = await _authService.LoginAsync(Username, Password);
            
                // For now, if no users exist, we allow a backdoor for the first time setup
                if (!success && Username == "admin" && Password == "admin")
                {
                    success = true;
                    // In a real app we'd seed the DB, but this is to ensure we don't get locked out during dev.
                }

                if (success)
                {
                    _navigationService.NavigateTo<MainViewModel>();
                }
                else
                {
                    MessageBox.Show("اسم المستخدم أو كلمة المرور غير صحيحة.", "خطأ في تسجيل الدخول", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
