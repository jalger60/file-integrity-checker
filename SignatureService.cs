using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileIntegrityChecker.WinForms.Services
{
    public static class SignatureService
    {
        public static string KeysDir = Path.Combine(AppContext.BaseDirectory, "Keys");

        public static (string privatePemPath, string publicPemPath) EnsureOrCreateKeyPair()
        {
            Directory.CreateDirectory(KeysDir);
            var privPath = Path.Combine(KeysDir, "rsa_private.pem");
            var pubPath = Path.Combine(KeysDir, "rsa_public.pem");

            if (!File.Exists(privPath) || !File.Exists(pubPath))
            {
                using var rsa = RSA.Create(2048);
                File.WriteAllText(privPath, ExportPrivateKeyPem(rsa));
                File.WriteAllText(pubPath, ExportPublicKeyPem(rsa));
            }
            return (privPath, pubPath);
        }

        public static string SignFile(string filePath)
        {
            var (privPem, _) = EnsureOrCreateKeyPair();
            using var rsa = RSA.Create();
            ImportPrivateKeyPem(rsa, File.ReadAllText(privPem));

            var data = File.ReadAllBytes(filePath);
            var sig = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var sigPath = filePath + ".sig";
            File.WriteAllBytes(sigPath, sig);
            return sigPath;
        }

        public static bool VerifyFile(string filePath)
        {
            var (_, pubPem) = EnsureOrCreateKeyPair();
            using var rsa = RSA.Create();
            ImportPublicKeyPem(rsa, File.ReadAllText(pubPem));

            var data = File.ReadAllBytes(filePath);
            var sigPath = filePath + ".sig";
            if (!File.Exists(sigPath)) return false;
            var sig = File.ReadAllBytes(sigPath);
            return rsa.VerifyData(data, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        // ---- PEM helpers ----

        private static string ExportPublicKeyPem(RSA rsa)
        {
            // In modern .NET, you can simply use: return rsa.ExportSubjectPublicKeyInfoPem();
            var der = rsa.ExportSubjectPublicKeyInfo();
            return ExportPem("PUBLIC KEY", der);
        }

        private static string ExportPrivateKeyPem(RSA rsa)
        {
            // In modern .NET, you can simply use: return rsa.ExportRSAPrivateKeyPem();
            var der = rsa.ExportRSAPrivateKey();
            return ExportPem("RSA PRIVATE KEY", der);
        }

        private static void ImportPublicKeyPem(RSA rsa, string pem)
        {
            // In modern .NET, you can simply use: rsa.ImportFromPem(pem);
            var base64 = new StringBuilder(pem)
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .ToString();
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(base64), out _);
        }

        private static void ImportPrivateKeyPem(RSA rsa, string pem)
        {
            // In modern .NET, you can simply use: rsa.ImportFromPem(pem);
            var base64 = new StringBuilder(pem)
                .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                .Replace("-----END RSA PRIVATE KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .ToString();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(base64), out _);
        }

        private static string ExportPem(string label, byte[] data)
        {
            var pem = new StringBuilder();
            pem.AppendLine($"-----BEGIN {label}-----");
            pem.AppendLine(Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks));
            pem.AppendLine($"-----END {label}-----");
            return pem.ToString();
        }
    }
}