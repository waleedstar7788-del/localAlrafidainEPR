using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LicenseGenerator.Services
{
    public class RsaKeyManager
    {
        private const int KeySizeBits = 2048;

        public (string privateKeyXml, string publicKeyXml) GenerateKeyPair()
        {
            using var rsa = RSA.Create(KeySizeBits);
            string privateKeyXml = rsa.ToXmlString(true);
            string publicKeyXml = rsa.ToXmlString(false);
            return (privateKeyXml, publicKeyXml);
        }

        public string SignData(string dataToSign, string privateKeyXml)
        {
            if (string.IsNullOrWhiteSpace(privateKeyXml))
            {
                throw new ArgumentException("المفتاح الخاص غير موجود أو غيرة صالح.");
            }

            using var rsa = RSA.Create();
            rsa.FromXmlString(privateKeyXml);

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
            byte[] signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signatureBytes);
        }

        public bool VerifyData(string dataToVerify, string signatureBase64, string publicKeyXml)
        {
            if (string.IsNullOrWhiteSpace(publicKeyXml) || string.IsNullOrWhiteSpace(signatureBase64))
            {
                return false;
            }

            try
            {
                using var rsa = RSA.Create();
                rsa.FromXmlString(publicKeyXml);

                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToVerify);
                byte[] signatureBytes = Convert.FromBase64String(signatureBase64);

                return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch
            {
                return false;
            }
        }
    }
}
