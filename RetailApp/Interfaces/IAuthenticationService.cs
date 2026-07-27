using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IAuthenticationService
    {
        AppUser? CurrentUser { get; }
        bool IsLoggedIn { get; }
        
        Task<bool> LoginAsync(string username, string password);
        void Logout();
        Task<bool> ChangePasswordAsync(string oldPassword, string newPassword);
    }
}
