using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileIntegrityChecker.WinForms.Models;


namespace FileIntegrityChecker.WinForms.Services
{
    public static class ReportExporter
    {
        public static string ExportCsv(IEnumerable<ScanResultRow> rows, string outputDir)
        {
            Directory.CreateDirectory(outputDir);
            var file = Path.Combine(outputDir, $"integrity_report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            var sb = new StringBuilder();
            sb.AppendLine("RelativePath,BaselineHash,CurrentHash,Status");
            foreach (var r in rows)
            {
                sb.AppendLine($"\"{r.RelativePath.Replace("\"", "\"\"")}\",{r.BaselineHash},{r.CurrentHash},{r.Status}");
            }
            File.WriteAllText(file, sb.ToString());
            return file;
        }
    }
}