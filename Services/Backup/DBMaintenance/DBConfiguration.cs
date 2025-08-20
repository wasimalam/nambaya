namespace DBMaintenance
{
    internal class DBConfiguration
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Databases { get; set; }
        public string BackupFolder { get; set; }
        public int DailyRetentionAfter { get; set; }
        public int WeeklyRetentionAfter { get; set; }
        public int MonthlyRetentionAfter { get; set; }
        public string NotificationAddress { get; set; }
        public int ScheduleAtHour { get; set; }
        public int ScheduleAtMinute { get; set; }
        public bool ScheduleSameDay { get; set; }
        public int WeeklyDay { get; set; }
    }
    internal enum Frequency
    {
        Daily=1,
        Weekly=7,
        Monthly=30
    }
}
