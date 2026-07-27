using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class FirstRunService : IFirstRunService
    {
        public async Task<bool> IsFirstRunRequiredAsync()
        {
            try
            {
                using var dbContext = new AppDbContext();
                // If we can't connect, or if Users table is empty, it's a first run
                if (!await dbContext.Database.CanConnectAsync())
                {
                    return true;
                }

                bool hasAdmin = await dbContext.AppUsers.AnyAsync();
                return !hasAdmin;
            }
            catch
            {
                // Typically implies database doesn't exist or isn't migrated
                return true;
            }
        }
    }
}
