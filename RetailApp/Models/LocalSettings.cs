namespace RetailApp.Models
{
    public class LocalSettings
    {
        // General
        public string DefaultLanguage { get; set; } = "ar"; // ar or en
        public bool IsRTL { get; set; } = true;
        public string Theme { get; set; } = "Light"; // Dark, Light, System
        public string AccentColor { get; set; } = "Blue";
        public bool AutoSave { get; set; } = true;
        public bool AutoRefresh { get; set; } = true;

        // Hardware & POS
        public string BarcodeScannerPort { get; set; } = "USB";
        public string CashDrawerPort { get; set; } = "COM1";
        public string ReceiptPrinterName { get; set; } = "Default";
        public string A4PrinterName { get; set; } = "Default";

        // Printing
        public string InvoiceTemplate { get; set; } = "Standard";
        public string LogoPath { get; set; } = "";
        public string FooterText { get; set; } = "شكراً لزيارتكم";
        public string HeaderText { get; set; } = "";
        public string Margins { get; set; } = "10,10,10,10";

        // Notifications
        public bool EnableDesktopNotifications { get; set; } = true;
        public bool EnableWarningMessages { get; set; } = true;
        public bool EnableSoundEffects { get; set; } = true;
    }
}
