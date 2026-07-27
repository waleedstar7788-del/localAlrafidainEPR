using System;
using System.Collections.ObjectModel;

namespace RetailApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Cashier { get; set; } = string.Empty;
        public Customer? Customer { get; set; }
        public string PaymentType { get; set; } = string.Empty; // Cash, Credit, Installment, Mixed
        public ObservableCollection<CartItem> Items { get; set; } = new ObservableCollection<CartItem>();
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
