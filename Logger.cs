using System;
using System.IO;


namespace FileIntegrityChecker.WinForms.Services
{
    public static class Logger
    {
        private static readonly object _sync = new();


        public static string LogDir = Path.Combine(AppContext.BaseDirectory, "Logs");


        public static string Log(string message)
        {
            lock (_sync)
            {
                Directory.CreateDirectory(LogDir);
                var path = Path.Combine(LogDir, $"integrity_log_{DateTime.Now:yyyyMMdd}.txt");
                File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
                return path;
            }
        }
    }
}