using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;

namespace RetailApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppUser? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var user = await context.AppUsers
                    .FirstOrDefaultAsync(u => u.Username == username && u.Status == UserStatus.Active);

                if (user == null) return false;

                var hashedInput = HashPassword(password);
                if (user.PasswordHash == hashedInput)
                {
                    CurrentUser = user;
                    user.LastLoginDate = DateTime.Now;
                    await context.SaveChangesAsync();

                    try
                    {
                        var auditLogService = scope.ServiceProvider.GetService<IAuditLogService>();
                        if (auditLogService != null)
                        {
                            await auditLogService.LogActionAsync("System", "Login", "User successfully logged in.");
                        }
                    }
                    catch { /* Don't let audit logging failure prevent login */ }

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Logout()
        {
            if (CurrentUser != null)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var auditLogService = scope.ServiceProvider.GetService<IAuditLogService>();
                    auditLogService?.LogActionAsync("System", "Logout", "User logged out.").Wait();
                }
                catch { /* Don't let audit logging failure prevent logout */ }
                CurrentUser = null;
            }
        }

        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            if (CurrentUser == null) return false;

            var hashedOld = HashPassword(oldPassword);
            if (CurrentUser.PasswordHash != hashedOld) return false;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            CurrentUser.PasswordHash = HashPassword(newPassword);
            context.AppUsers.Update(CurrentUser);
            await context.SaveChangesAsync();

            try
            {
                var auditLogService = scope.ServiceProvider.GetService<IAuditLogService>();
                if (auditLogService != null)
                {
                    await auditLogService.LogActionAsync("Security", "ChangePassword", "User changed their password.");
                }
            }
            catch { /* Don't let audit logging failure prevent password change */ }

            return true;
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
