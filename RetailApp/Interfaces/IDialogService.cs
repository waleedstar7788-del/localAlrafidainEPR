using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IDialogService
    {
        Task<bool> ShowDialogAsync(string viewName, object? viewModel);
        Task<bool> ShowConfirmationAsync(string message);
    }
}
