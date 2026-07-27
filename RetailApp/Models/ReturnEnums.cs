namespace RetailApp.Models
{
    public enum ReturnStatus
    {
        Draft,
        Pending,
        Completed,
        Cancelled
    }

    public enum ReturnReason
    {
        Damaged,
        Expired,
        WrongItem,
        CustomerRequest,
        SupplierError,
        Other
    }

    public enum RefundMethod
    {
        CashRefund,
        Credit,       // Customer/Supplier Balance
        Exchange
    }
}
