using System;

namespace LicenseGenerator.Models
{
    public enum LicenseType
    {
        Trial,
        Monthly,
        Quarterly,
        Yearly,
        Lifetime,
        Custom
    }

    public enum ActivationMode
    {
        FixedDates,
        FromFirstUse
    }

    public enum LicenseStatus
    {
        Active,
        Expired,
        Suspended,
        Disabled,
        Trial,
        Pending,
        Invalid,
        InvalidSignature,
        MachineMismatch,
        ClockRollbackDetected,
        Missing
    }

    public class LicenseData
    {
        public int Version { get; set; } = 1;
        public string LicenseKey { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string MachineId { get; set; } = string.Empty;
        public int MaxDevices { get; set; } = 1;
        public ActivationMode Mode { get; set; } = ActivationMode.FixedDates;
        public int DurationDays { get; set; } = 30;
        public LicenseType SubscriptionType { get; set; } = LicenseType.Yearly;
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime ActivationDate { get; set; } = DateTime.Now;
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddYears(1);
        public string ProductVersion { get; set; } = "1.0.0";
        public LicenseStatus Status { get; set; } = LicenseStatus.Active;
        public string Signature { get; set; } = string.Empty;

        public string GetCanonicalPayload()
        {
            var issueUtc = IssueDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var expUtc = ExpirationDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            
            return $"CustomerName={CustomerName?.Trim()}|CompanyName={CompanyName?.Trim()}|MachineId={MachineId?.Trim()}|MaxDevices={MaxDevices}|Mode={Mode}|DurationDays={DurationDays}|SubscriptionType={SubscriptionType}|IssueDate={issueUtc}|ExpirationDate={expUtc}|ProductVersion={ProductVersion?.Trim()}";
        }
    }
}
