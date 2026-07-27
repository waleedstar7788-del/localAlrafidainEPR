namespace RetailApp.Models
{
    public class AppSettings
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = "My Store";
        public string ArabicName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string StoreLogo { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string CommercialRegistration { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public string Timezone { get; set; } = "UTC";
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // Accounting
        public decimal Tax { get; set; } = 0;
        public string FiscalYear { get; set; } = "2026";
        public string DefaultPaymentMethod { get; set; } = "Cash";

        // Inventory
        public int LowStockThreshold { get; set; } = 5;
        public bool AllowNegativeStock { get; set; } = false;
        public bool AutoGenerateSKU { get; set; } = true;
        public bool AutoGenerateBarcode { get; set; } = true;

        // Security
        public string PasswordPolicyLevel { get; set; } = "Medium";
        public int SessionTimeoutMinutes { get; set; } = 30;
        public bool AutoLogoutEnabled { get; set; } = true;
        public int MaxLoginAttempts { get; set; } = 5;

        // Sales/POS
        public int? DefaultWarehouseId { get; set; }
        public int? DefaultCustomerId { get; set; }
        public bool AutoSaveInvoice { get; set; } = true;

        // Legacy / Migrated to LocalSettings but kept for DB compat if needed
        public double ReceiptWidth { get; set; } = 80.0;
        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public string TimeFormat { get; set; } = "HH:mm:ss";
        public string Language { get; set; } = "English";
        public string Theme { get; set; } = "Dark";
        public string BackupLocation { get; set; } = string.Empty;
    }
}
