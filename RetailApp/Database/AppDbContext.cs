using Microsoft.EntityFrameworkCore;
using RetailApp.Models;
using System;
using System.IO;

namespace RetailApp.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AppSettings> Settings { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesItem> SalesItems { get; set; }
        public DbSet<SalesReturnInvoice> SalesReturnInvoices { get; set; }
        public DbSet<SalesReturnItem> SalesReturnItems { get; set; }
        public DbSet<PurchaseReturnInvoice> PurchaseReturnInvoices { get; set; }
        public DbSet<PurchaseReturnItem> PurchaseReturnItems { get; set; }
        
        public DbSet<InstallmentContract> InstallmentContracts { get; set; }
        public DbSet<InstallmentSchedule> InstallmentSchedules { get; set; }
        public DbSet<InstallmentPayment> InstallmentPayments { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

        public DbSet<ExpenseEntry> ExpenseEntries { get; set; }
        public DbSet<IncomeEntry> IncomeEntries { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAdvance> EmployeeAdvances { get; set; }
        public DbSet<PayrollRecord> PayrollRecords { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerNumber)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone1)
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.SupplierNumber)
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Phone1)
                .IsUnique();

            modelBuilder.Entity<PurchaseInvoice>()
                .HasIndex(p => p.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<SalesInvoice>()
                .HasIndex(s => s.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<SalesReturnInvoice>()
                .HasIndex(s => s.ReturnNumber)
                .IsUnique();

            modelBuilder.Entity<PurchaseReturnInvoice>()
                .HasIndex(p => p.ReturnNumber)
                .IsUnique();

            modelBuilder.Entity<InstallmentContract>()
                .HasIndex(i => i.ContractNumber)
                .IsUnique();

            modelBuilder.Entity<JournalEntry>()
                .HasIndex(j => j.JournalNumber)
                .IsUnique();

            modelBuilder.Entity<ExpenseEntry>()
                .HasIndex(e => e.ExpenseNumber)
                .IsUnique();

            modelBuilder.Entity<IncomeEntry>()
                .HasIndex(i => i.IncomeNumber)
                .IsUnique();

            modelBuilder.Entity<EmployeeAdvance>()
                .HasIndex(a => a.AdvanceNumber)
                .IsUnique();

            modelBuilder.Entity<PayrollRecord>()
                .HasIndex(p => p.PayrollNumber)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeNumber)
                .IsUnique();

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(localAppData, "RetailApp");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            string dbPath = Path.Combine(appFolder, "app.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // SQLite does not support decimal properly for aggregation (SUM). 
            // Convert all decimals to double at the database level.
            configurationBuilder.Properties<decimal>().HaveConversion<double>();
        }
    }
}
