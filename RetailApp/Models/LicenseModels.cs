using System;

namespace RetailApp.Models
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
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime ActivationDate { get; set; } = DateTime.Now;
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddYears(1);
        public string MachineId { get; set; } = string.Empty;
        public int MaxDevices { get; set; } = 1;
        public ActivationMode Mode { get; set; } = ActivationMode.FixedDates;
        public int DurationDays { get; set; } = 30;
        public string ProductVersion { get; set; } = "1.0.0";
        public string AppVersion { get; set; } = "1.0.0";
        public string Edition { get; set; } = "Enterprise";
        public LicenseType SubscriptionType { get; set; } = LicenseType.Yearly;
        public LicenseStatus Status { get; set; } = LicenseStatus.Pending;
        public string Signature { get; set; } = string.Empty;

        public string GetCanonicalPayload()
        {
            var issueDateToUse = IssueDate != default ? IssueDate : ActivationDate;
            var issueUtc = issueDateToUse.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var expUtc = ExpirationDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var versionToUse = !string.IsNullOrWhiteSpace(ProductVersion) ? ProductVersion : AppVersion;
            
            return $"CustomerName={CustomerName?.Trim()}|CompanyName={CompanyName?.Trim()}|MachineId={MachineId?.Trim()}|MaxDevices={MaxDevices}|Mode={Mode}|DurationDays={DurationDays}|SubscriptionType={SubscriptionType}|IssueDate={issueUtc}|ExpirationDate={expUtc}|ProductVersion={versionToUse?.Trim()}";
        }

        public int RemainingDays
        {
            get
            {
                if (SubscriptionType == LicenseType.Lifetime) return 9999;
                var remaining = (ExpirationDate.Date - DateTime.Now.Date).Days;
                return remaining > 0 ? remaining : 0;
            }
        }
    }

    public class UpdateInfo
    {
        public string Version { get; set; } = string.Empty;
        public string BuildNumber { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string ReleaseNotes { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
    }
}
