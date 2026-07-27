using System;
using System.Security.Cryptography;
using System.Text;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class LicenseValidationService : ILicenseValidationService
    {
        public bool ValidateSignature(LicenseData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Signature))
            {
                return false;
            }

            try
            {
                string canonicalPayload = data.GetCanonicalPayload();
                byte[] dataBytes = Encoding.UTF8.GetBytes(canonicalPayload);
                byte[] signatureBytes = Convert.FromBase64String(data.Signature);

                using var rsa = RSA.Create();
                rsa.FromXmlString(LicenseKeys.RsaPublicKeyXml);

                if (rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {
                    return true;
                }

                // Fallback SHA256 signature validation (for local trial/dev licenses)
                using var sha = SHA256.Create();
                byte[] hash = sha.ComputeHash(dataBytes);
                string expectedHashSig = Convert.ToBase64String(hash);
                return data.Signature == expectedHashSig;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateLicenseKey()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
        }

        public string GenerateSignature(LicenseData data)
        {
            if (data == null) return string.Empty;
            try
            {
                string canonicalPayload = data.GetCanonicalPayload();
                byte[] dataBytes = Encoding.UTF8.GetBytes(canonicalPayload);
                using var sha = SHA256.Create();
                byte[] hash = sha.ComputeHash(dataBytes);
                return Convert.ToBase64String(hash);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
