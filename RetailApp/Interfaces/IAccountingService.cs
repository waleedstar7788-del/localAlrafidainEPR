using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IAccountingService
    {
        Task SeedDefaultAccountsAsync();
        Task<List<Account>> GetAllAccountsAsync();
        Task<Account> CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(int id);
        Task<decimal> GetAccountBalanceAsync(int accountId);
    }
}
