using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileIntegrityChecker.WinForms.Models;


namespace FileIntegrityChecker.WinForms.Services
{
    public static class Scanner
    {
        public static List<ScanResultRow> CompareToBaseline(IntegrityStore baseline)
        {
            var results = new List<ScanResultRow>();
            var root = baseline.RootDirectory;
            var baselineMap = baseline.Records.ToDictionary(r => r.RelativePath, r => r.Sha256);


            // Current files
            var currentFiles = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(f => new
            {
                Rel = Path.GetRelativePath(root, f),
                Hash = HashService.ComputeSha256(f)
            })
            .ToDictionary(x => x.Rel, x => x.Hash);


            // Check existing/missing
            foreach (var kv in baselineMap)
            {
                var rel = kv.Key;
                var baseHash = kv.Value;
                if (currentFiles.TryGetValue(rel, out var curHash))
                {
                    results.Add(new ScanResultRow
                    {
                        RelativePath = rel,
                        BaselineHash = baseHash,
                        CurrentHash = curHash,
                        Status = curHash == baseHash ? IntegrityStatus.Unchanged : IntegrityStatus.Modified
                    });
                }
                else
                {
                    results.Add(new ScanResultRow
                    {
                        RelativePath = rel,
                        BaselineHash = baseHash,
                        CurrentHash = string.Empty,
                        Status = IntegrityStatus.Missing
                    });
                }
            }


            // New files
            foreach (var kv in currentFiles)
            {
                if (!baselineMap.ContainsKey(kv.Key))
                {
                    results.Add(new ScanResultRow
                    {
                        RelativePath = kv.Key,
                        BaselineHash = string.Empty,
                        CurrentHash = kv.Value,
                        Status = IntegrityStatus.New
                    });
                }
            }


            return results.OrderBy(r => r.RelativePath).ToList();
        }
    }
}