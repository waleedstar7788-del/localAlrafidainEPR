using RetailApp.Interfaces;
using System.Windows;

namespace RetailApp.Services
{
    public class NotificationService : INotificationService
    {
        public void ShowSuccess(string message)
        {
            MessageBox.Show(message, "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "معلومة", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
