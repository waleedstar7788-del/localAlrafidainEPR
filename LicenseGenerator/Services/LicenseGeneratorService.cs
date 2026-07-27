using System;
using System.IO;
using System.Text.Json;
using LicenseGenerator.Models;

namespace LicenseGenerator.Services
{
    public class LicenseGeneratorService
    {
        private readonly RsaKeyManager _rsaKeyManager;

        public LicenseGeneratorService(RsaKeyManager rsaKeyManager)
        {
            _rsaKeyManager = rsaKeyManager;
        }

        public string GenerateAndSignLicense(LicenseData license, string privateKeyXml)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));
            if (string.IsNullOrWhiteSpace(privateKeyXml)) throw new InvalidOperationException("يجب توفير المفتاح الخاص RSA للتوقيع.");

            string canonicalPayload = license.GetCanonicalPayload();
            license.Signature = _rsaKeyManager.SignData(canonicalPayload, privateKeyXml);

            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(license, options);
        }

        public void SaveLicenseToFile(LicenseData license, string privateKeyXml, string filePath)
        {
            string json = GenerateAndSignLicense(license, privateKeyXml);
            File.WriteAllText(filePath, json);
        }

        public string GenerateLicenseCode(LicenseData license, string privateKeyXml)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));
            return CompactKeyService.EncodeCompactCode(license);
        }

        public (bool isValid, LicenseData? data) VerifyLicenseFile(string filePath, string publicKeyXml)
        {
            if (!File.Exists(filePath)) return (false, null);

            try
            {
                string json = File.ReadAllText(filePath);
                var license = JsonSerializer.Deserialize<LicenseData>(json);
                if (license == null || string.IsNullOrEmpty(license.Signature))
                {
                    return (false, null);
                }

                string payload = license.GetCanonicalPayload();
                bool isValid = _rsaKeyManager.VerifyData(payload, license.Signature, publicKeyXml);
                return (isValid, license);
            }
            catch
            {
                return (false, null);
            }
        }
    }
}
