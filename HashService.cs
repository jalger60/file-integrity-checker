using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace FileIntegrityChecker.WinForms.Services
{
    public static class HashService
    {
        public static string ComputeSha256(string path)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(path);
            var hash = sha.ComputeHash(stream);
            return Convert.ToHexString(hash); // uppercase hex
        }


        public static string NormalizeDirectory(string dir)
        {
            return Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}