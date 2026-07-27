using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace RetailApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _busyMessage = "جاري التحميل...";

        [RelayCommand]
        public void Close(object? parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
