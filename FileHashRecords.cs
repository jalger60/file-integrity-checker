using System;


namespace FileIntegrityChecker.WinForms.Models
{
    public class FileHashRecord
    {
        public string RelativePath { get; set; } = string.Empty; // path relative to baseline root
        public string Sha256 { get; set; } = string.Empty;
        public DateTimeOffset LastWriteUtc { get; set; }
    }
}