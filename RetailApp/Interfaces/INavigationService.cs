using CommunityToolkit.Mvvm.ComponentModel;

namespace RetailApp.Interfaces
{
    public interface INavigationService
    {
        ObservableObject? CurrentView { get; }
        void NavigateTo<TViewModel>() where TViewModel : ObservableObject;
    }
}
