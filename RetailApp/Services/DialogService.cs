using RetailApp.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RetailApp.Services
{
    public class DialogService : IDialogService
    {
        public Task<bool> ShowDialogAsync(string viewName, object? viewModel)
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    // Find the UserControl by name
                    Type? type = Assembly.GetExecutingAssembly().GetType($"RetailApp.Controls.{viewName}");
                    if (type == null)
                    {
                        MessageBox.Show($"Could not find view: {viewName}");
                        tcs.SetResult(false);
                        return;
                    }

                    var content = Activator.CreateInstance(type) as UserControl;
                    if (content != null)
                    {
                        content.DataContext = viewModel;
                    }

                    // Create a generic window to host it
                    var window = new Window
                    {
                        Content = content,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        WindowStyle = WindowStyle.None,
                        AllowsTransparency = true,
                        Background = System.Windows.Media.Brushes.Transparent,
                        ShowInTaskbar = false
                    };

                    // Handle window closed
                    window.Closed += (s, e) =>
                    {
                        bool result = window.DialogResult ?? false;
                        tcs.TrySetResult(result);
                    };

                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    tcs.TrySetResult(false);
                }
            });

            return tcs.Task;
        }

        public Task<bool> ShowConfirmationAsync(string message)
        {
            var result = MessageBox.Show(message, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return Task.FromResult(result == MessageBoxResult.Yes);
        }
    }
}
