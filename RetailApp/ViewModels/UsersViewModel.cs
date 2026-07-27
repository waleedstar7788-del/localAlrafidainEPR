using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using RetailApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetailApp.ViewModels
{
    public partial class UsersViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly IAuthenticationService _authService;

        [ObservableProperty] private ObservableCollection<AppUser> _users = new();
        [ObservableProperty] private ObservableCollection<AuditLog> _auditLogs = new();

        public UsersViewModel(AppDbContext context, IDialogService dialogService, INotificationService notificationService, IAuthenticationService authService)
        {
            _context = context;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _authService = authService;
            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            await LoadUsersAsync();
            await LoadAuditLogsAsync();
        }

        private async Task LoadUsersAsync()
        {
            Users.Clear();
            var list = await _context.AppUsers.ToListAsync();
            foreach (var u in list) Users.Add(u);
        }

        private async Task LoadAuditLogsAsync()
        {
            AuditLogs.Clear();
            var list = await _context.AuditLogs.OrderByDescending(l => l.Timestamp).Take(100).ToListAsync();
            foreach (var l in list) AuditLogs.Add(l);
        }

        [RelayCommand]
        private async Task AddUserAsync()
        {
            // Placeholder for UserEditorDialog
            _notificationService.ShowSuccess("فتح نافذة إضافة مستخدم.");
        }

        [RelayCommand]
        private async Task EditUserAsync(AppUser user)
        {
            if (user == null) return;
            // Placeholder for UserEditorDialog
            _notificationService.ShowSuccess("فتح نافذة تعديل مستخدم.");
        }

        [RelayCommand]
        private async Task DeleteUserAsync(AppUser user)
        {
            if (user == null) return;
            if (user.Username == "admin")
            {
                _notificationService.ShowError("لا يمكن حذف مدير النظام.");
                return;
            }

            bool confirm = await _dialogService.ShowConfirmationAsync($"هل أنت متأكد من حذف المستخدم {user.FullName}؟");
            if (confirm)
            {
                _context.AppUsers.Remove(user);
                await _context.SaveChangesAsync();
                await LoadUsersAsync();
                _notificationService.ShowSuccess("تم الحذف بنجاح.");
            }
        }
    }
}
