using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;

using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private ObservableObject? _currentView;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ObservableObject? CurrentView
        {
            get => _currentView;
            private set => SetProperty(ref _currentView, value);
        }

        public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
        {
            CurrentView = _serviceProvider.GetRequiredService<TViewModel>();
        }
    }
}
