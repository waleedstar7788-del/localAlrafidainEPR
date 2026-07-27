using System;

namespace RetailApp.Models
{
    public enum BackupType
    {
        Full,
        Manual,
        Automatic,
        Scheduled,
        Quick,
        OnStartup,
        OnExit
    }

    public enum BackupStatus
    {
        Success,
        Failed,
        InProgress
    }

    public enum BackupFrequency
    {
        None,
        Daily,
        Weekly,
        Monthly,
        OnStartup,
        OnExit
    }

    public class BackupHistoryItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Date { get; set; }
        public string User { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public TimeSpan Duration { get; set; }
        public BackupStatus Status { get; set; }
        public string Location { get; set; } = string.Empty;
        public BackupType BackupType { get; set; }
        public bool RestoreAvailable { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class BackupScheduleConfig
    {
        public BackupFrequency Frequency { get; set; } = BackupFrequency.None;
        public int MaxRetainedBackups { get; set; } = 10;
        public TimeSpan ScheduledTime { get; set; } = new TimeSpan(2, 0, 0); // Default 2:00 AM
        public DayOfWeek ScheduledDayOfWeek { get; set; } = DayOfWeek.Friday;
        public int ScheduledDayOfMonth { get; set; } = 1;
        public DateTime? LastRun { get; set; }
    }

    public class BackupMetadata
    {
        public string AppVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public BackupType BackupType { get; set; }
        public long DatabaseSizeBytes { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
