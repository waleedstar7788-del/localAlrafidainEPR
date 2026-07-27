namespace RetailApp.Models
{
    public enum AccountCategory
    {
        Asset,        // 1xxx
        Liability,    // 2xxx
        Equity,       // 3xxx
        Revenue,      // 4xxx
        Expense       // 5xxx
    }

    public enum JournalStatus
    {
        Draft,
        Posted,
        Cancelled
    }
}
