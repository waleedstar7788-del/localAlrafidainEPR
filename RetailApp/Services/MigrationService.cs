using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RetailApp.Database;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class MigrationService : IMigrationService
    {
        public MigrationService()
        {
        }

        public async Task MigrateDatabaseAsync()
        {
            using var dbContext = new AppDbContext();
            await dbContext.Database.MigrateAsync();
        }

        public async Task SeedDefaultDataAsync(string adminUsername, string adminPassword, string companyName)
        {
            using var dbContext = new AppDbContext();

            // Seed AppSettings
            if (!await dbContext.Settings.AnyAsync())
            {
                dbContext.Settings.Add(new AppSettings
                {
                    StoreName = companyName,
                    ArabicName = companyName,
                    Currency = "SAR"
                });
            }

            // Seed Admin User
            if (!await dbContext.AppUsers.AnyAsync(u => u.Username == adminUsername))
            {
                var hash = AuthenticationService.HashPassword(adminPassword);

                var adminUser = new AppUser
                {
                    Username = adminUsername,
                    PasswordHash = hash,
                    FullName = "مدير النظام",
                    Status = UserStatus.Active,
                    CreatedDate = DateTime.Now
                };
                dbContext.AppUsers.Add(adminUser);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
