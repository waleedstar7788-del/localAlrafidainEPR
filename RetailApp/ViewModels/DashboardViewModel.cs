using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private bool _isLoading = true;

        [ObservableProperty]
        private string _storeName = "My Store (Mock)";

        [ObservableProperty]
        private string _currentUserName = "Admin User";

        [ObservableProperty]
        private string _currentDate = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

        public ObservableCollection<StatItem> Stats { get; } = new ObservableCollection<StatItem>();
        public ObservableCollection<ActivityItem> RecentActivities { get; } = new ObservableCollection<ActivityItem>();
        public ObservableCollection<NotificationItem> Notifications { get; } = new ObservableCollection<NotificationItem>();
        public ObservableCollection<SystemStatusItem> SystemStatusItems { get; } = new ObservableCollection<SystemStatusItem>();

        [ObservableProperty]
        private ISeries[]? _dailySalesSeries;

        [ObservableProperty]
        private ISeries[]? _monthlySalesSeries;

        [ObservableProperty]
        private ISeries[]? _purchasesVsSalesSeries;

        [ObservableProperty]
        private ISeries[]? _salesByCategorySeries;

        public DashboardViewModel(IStatisticsService statisticsService, IServiceProvider serviceProvider)
        {
            _statisticsService = statisticsService;
            _serviceProvider = serviceProvider;
            _ = InitializeAsync();
        }

        [RelayCommand]
        private void CreateInvoice()
        {
            if (_serviceProvider.GetService(typeof(MainViewModel)) is MainViewModel mainViewModel)
            {
                mainViewModel.NavigateToPosCommand.Execute(null);
            }
        }

        private async Task InitializeAsync()
        {
            IsLoading = true;

            try
            {
                var statsTask = _statisticsService.GetDashboardStatsAsync();
                var activitiesTask = _statisticsService.GetRecentActivitiesAsync();
                var notificationsTask = _statisticsService.GetNotificationsAsync();
                var statusTask = _statisticsService.GetSystemStatusAsync();
                var dailyChartTask = _statisticsService.GetDailySalesChartAsync();
                var monthlyChartTask = _statisticsService.GetMonthlySalesChartAsync();
                var pvssTask = _statisticsService.GetPurchasesVsSalesChartAsync();
                var categoryTask = _statisticsService.GetSalesByCategoryChartAsync();

                await Task.WhenAll(
                    statsTask, activitiesTask, notificationsTask, statusTask,
                    dailyChartTask, monthlyChartTask, pvssTask, categoryTask
                );

                foreach (var s in await statsTask) Stats.Add(s);
                foreach (var a in await activitiesTask) RecentActivities.Add(a);
                foreach (var n in await notificationsTask) Notifications.Add(n);
                foreach (var st in await statusTask) SystemStatusItems.Add(st);

                DailySalesSeries = await dailyChartTask;
                MonthlySalesSeries = await monthlyChartTask;
                PurchasesVsSalesSeries = await pvssTask;
                SalesByCategorySeries = await categoryTask;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
