using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailApp.Models
{
    public class PrintTemplate : INotifyPropertyChanged
    {
        private string _id = Guid.NewGuid().ToString();
        public string Id 
        { 
            get => _id; 
            set { _id = value; OnPropertyChanged(); } 
        }

        private string _name = "قالب جديد";
        public string Name 
        { 
            get => _name; 
            set { _name = value; OnPropertyChanged(); } 
        }

        private string _documentType = "SalesInvoice"; // SalesInvoice, PurchaseInvoice, Receipt, ReturnInvoice
        public string DocumentType 
        { 
            get => _documentType; 
            set { _documentType = value; OnPropertyChanged(); } 
        }

        // Layout Settings
        private string _paperSize = "80mm"; // 80mm, 58mm, A4, A5
        public string PaperSize 
        { 
            get => _paperSize; 
            set { _paperSize = value; OnPropertyChanged(); } 
        }

        private string _margins = "10,10,10,10";
        public string Margins 
        { 
            get => _margins; 
            set { _margins = value; OnPropertyChanged(); } 
        }

        private double _receiptWidth = 280; // WPF Pixels roughly matching 80mm
        public double ReceiptWidth 
        { 
            get => _receiptWidth; 
            set { _receiptWidth = value; OnPropertyChanged(); } 
        }

        // Content Visibility Settings
        private string _logoPath = "";
        public string LogoPath { get => _logoPath; set { _logoPath = value; OnPropertyChanged(); } }

        private bool _showLogo = true;
        public bool ShowLogo { get => _showLogo; set { _showLogo = value; OnPropertyChanged(); } }

        private bool _showCompanyDetails = true;
        public bool ShowCompanyDetails { get => _showCompanyDetails; set { _showCompanyDetails = value; OnPropertyChanged(); } }

        private bool _showHeader = true;
        public bool ShowHeader { get => _showHeader; set { _showHeader = value; OnPropertyChanged(); } }

        private bool _showFooter = true;
        public bool ShowFooter { get => _showFooter; set { _showFooter = value; OnPropertyChanged(); } }

        private bool _showQR = true;
        public bool ShowQR { get => _showQR; set { _showQR = value; OnPropertyChanged(); } }

        private bool _showBarcode = false;
        public bool ShowBarcode { get => _showBarcode; set { _showBarcode = value; OnPropertyChanged(); } }

        // Company Info Overrides
        private string _companyName = "";
        public string CompanyName { get => _companyName; set { _companyName = value; OnPropertyChanged(); } }

        private string _companyArabicName = "";
        public string CompanyArabicName { get => _companyArabicName; set { _companyArabicName = value; OnPropertyChanged(); } }

        private string _companyAddress = "";
        public string CompanyAddress { get => _companyAddress; set { _companyAddress = value; OnPropertyChanged(); } }

        private string _companyPhone = "";
        public string CompanyPhone { get => _companyPhone; set { _companyPhone = value; OnPropertyChanged(); } }

        private string _companyMobile = "";
        public string CompanyMobile { get => _companyMobile; set { _companyMobile = value; OnPropertyChanged(); } }

        private string _companyEmail = "";
        public string CompanyEmail { get => _companyEmail; set { _companyEmail = value; OnPropertyChanged(); } }

        private string _companyWebsite = "";
        public string CompanyWebsite { get => _companyWebsite; set { _companyWebsite = value; OnPropertyChanged(); } }

        private string _companyTaxNumber = "";
        public string CompanyTaxNumber { get => _companyTaxNumber; set { _companyTaxNumber = value; OnPropertyChanged(); } }

        private string _companyCR = "";
        public string CompanyCR { get => _companyCR; set { _companyCR = value; OnPropertyChanged(); } }

        // Content Text
        private string _invoicePrefix = "INV-";
        public string InvoicePrefix { get => _invoicePrefix; set { _invoicePrefix = value; OnPropertyChanged(); } }

        private string _dateFormat = "yyyy/MM/dd";
        public string DateFormat { get => _dateFormat; set { _dateFormat = value; OnPropertyChanged(); } }

        private string _currencyDisplay = "د.ع";
        public string CurrencyDisplay { get => _currencyDisplay; set { _currencyDisplay = value; OnPropertyChanged(); } }

        // Sizing & Styles
        private double _logoSize = 80;
        public double LogoSize { get => _logoSize; set { _logoSize = value; OnPropertyChanged(); } }


        private double _headerHeight = 150;
        public double HeaderHeight { get => _headerHeight; set { _headerHeight = value; OnPropertyChanged(); } }

        private double _footerHeight = 100;
        public double FooterHeight { get => _footerHeight; set { _footerHeight = value; OnPropertyChanged(); } }

        private string _textAlignment = "Right";
        public string TextAlignment { get => _textAlignment; set { _textAlignment = value; OnPropertyChanged(); } }
        private string _invoiceTitle = "فاتورة مبيعات";
        public string InvoiceTitle { get => _invoiceTitle; set { _invoiceTitle = value; OnPropertyChanged(); } }

        private string _headerText = "أهلاً وسهلاً بكم";
        public string HeaderText { get => _headerText; set { _headerText = value; OnPropertyChanged(); } }

        private string _footerText = "شكراً لتسوقكم معنا";
        public string FooterText { get => _footerText; set { _footerText = value; OnPropertyChanged(); } }

        // Typography Settings
        private string _fontFamily = "Cairo";
        public string FontFamily { get => _fontFamily; set { _fontFamily = value; OnPropertyChanged(); } }

        private int _fontSize = 12;
        public int FontSize { get => _fontSize; set { _fontSize = value; OnPropertyChanged(); } }

        private bool _isBold = false;
        public bool IsBold { get => _isBold; set { _isBold = value; OnPropertyChanged(); } }

        private bool _isItalic = false;
        public bool IsItalic { get => _isItalic; set { _isItalic = value; OnPropertyChanged(); } }

        private bool _isUnderlined = false;
        public bool IsUnderlined { get => _isUnderlined; set { _isUnderlined = value; OnPropertyChanged(); } }

        // Colors
        private string _primaryColor = "#0F172A";
        public string PrimaryColor { get => _primaryColor; set { _primaryColor = value; OnPropertyChanged(); } }
        
        private string _secondaryColor = "#334155";
        public string SecondaryColor { get => _secondaryColor; set { _secondaryColor = value; OnPropertyChanged(); } }

        private string _borderColor = "#CBD5E1";
        public string BorderColor { get => _borderColor; set { _borderColor = value; OnPropertyChanged(); } }

        // Table Settings
        private bool _showItemBarcode = true;
        public bool ShowItemBarcode { get => _showItemBarcode; set { _showItemBarcode = value; OnPropertyChanged(); } }
        
        private bool _showItemDiscount = false;
        public bool ShowItemDiscount { get => _showItemDiscount; set { _showItemDiscount = value; OnPropertyChanged(); } }

        private bool _showTaxColumn = true;
        public bool ShowTaxColumn { get => _showTaxColumn; set { _showTaxColumn = value; OnPropertyChanged(); } }

        private bool _showDiscountColumn = true;
        public bool ShowDiscountColumn { get => _showDiscountColumn; set { _showDiscountColumn = value; OnPropertyChanged(); } }

        private bool _alternatingRowColors = true;
        public bool AlternatingRowColors { get => _alternatingRowColors; set { _alternatingRowColors = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PrintTemplate Clone()
        {
            return (PrintTemplate)this.MemberwiseClone();
        }
    }
}
