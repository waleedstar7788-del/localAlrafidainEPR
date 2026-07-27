namespace RetailApp.Models
{
    public enum InstallmentType
    {
        Monthly,
        Weekly,
        Daily,
        Custom
    }

    public enum ContractStatus
    {
        Pending,
        Active,
        Completed,
        Cancelled
    }

    public enum ScheduleStatus
    {
        Pending,
        Partial,
        Paid,
        Late,
        Overdue
    }

    public enum InstallmentPaymentMethod
    {
        Cash,
        BankTransfer,
        CreditCard,
        Mixed
    }
}
