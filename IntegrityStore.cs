using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FileIntegrityChecker.WinForms.Models;


namespace FileIntegrityChecker.WinForms.Services
{
    public class IntegrityStore
    {
        public string RootDirectory { get; set; } = string.Empty; // normalized absolute path
        public List<FileHashRecord> Records { get; set; } = new();


        public static IntegrityStore CreateBaseline(string rootDir, Func<string, bool>? includeFilter = null)
        {
            var store = new IntegrityStore { RootDirectory = HashService.NormalizeDirectory(rootDir) };
            includeFilter ??= (p) => true;


            foreach (var file in Directory.EnumerateFiles(store.RootDirectory, "*", SearchOption.AllDirectories))
            {
                if (!includeFilter(file)) continue;
                var rel = Path.GetRelativePath(store.RootDirectory, file);
                var rec = new FileHashRecord
                {
                    RelativePath = rel,
                    Sha256 = HashService.ComputeSha256(file),
                    LastWriteUtc = File.GetLastWriteTimeUtc(file)
                };
                store.Records.Add(rec);
            }
            return store;
        }


        public void Save(string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, options);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, json);
        }


        public static IntegrityStore Load(string path)
        {
            var json = File.ReadAllText(path);
            var store = JsonSerializer.Deserialize<IntegrityStore>(json)!;
            store.RootDirectory = HashService.NormalizeDirectory(store.RootDirectory);
            return store;
        }
    }
}