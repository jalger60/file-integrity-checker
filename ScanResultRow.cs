namespace FileIntegrityChecker.WinForms.Models
{
    public enum IntegrityStatus { Unchanged, Modified, New, Missing }


    public class ScanResultRow
    {
        public string RelativePath { get; set; } = string.Empty;
        public string CurrentHash { get; set; } = string.Empty;
        public string BaselineHash { get; set; } = string.Empty;
        public IntegrityStatus Status { get; set; }
    }
}