using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using RetailApp.Models;

namespace RetailApp.Services
{
    /// <summary>
    /// الخدمة الموحدة فائقة الضغط لتوليد وفك تشفير كود التفعيل النصي المرمز بأقصر صيغة ممكنة (RA-XXXXX-XXXXX...)
    /// تقوم بحقن كافة البيانات (اسم العميل، اسم الشركة، نوع الاشتراك، مدة الأيام، عدد الأجهزة، ومعرف الجهاز)
    /// مع التوقيع الرقمي HMAC-SHA256 وضغط البيانات باستخدام Deflate.
    /// </summary>
    public static class CompactKeyService
    {
        private const string Base32Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
        private static readonly byte[] HmacSecretKey = Encoding.UTF8.GetBytes("RetailApp-SuperSecret-HmacKey-2026!@#AlRafidainERP");

        public static string EncodeCompactCode(LicenseData license)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));

            byte typeAndMode = (byte)(((byte)license.SubscriptionType & 0x0F) | (((byte)license.Mode & 0x0F) << 4));
            ushort maxDev = license.MaxDevices <= 0 ? (ushort)0xFFFF : (ushort)Math.Min(license.MaxDevices, 65534);
            ushort durDays = (ushort)Math.Clamp(license.DurationDays > 0 ? license.DurationDays : 30, 1, 65535);

            uint mHash = 0;
            if (!string.IsNullOrWhiteSpace(license.MachineId) && license.MachineId.Trim() != "*")
            {
                mHash = ComputeCrc32(license.MachineId.Trim().ToUpperInvariant());
            }

            string custName = (license.CustomerName ?? "").Trim();
            string compName = (license.CompanyName ?? "").Trim();

            // Skip encoding default strings to achieve maximum code compactness!
            if (custName == "عميل الرافدين") custName = "";
            if (compName == "الرافدين ERP") compName = "";

            byte[] custBytes = Encoding.UTF8.GetBytes(custName);
            byte[] compBytes = Encoding.UTF8.GetBytes(compName);

            using var ms = new MemoryStream();
            using (var writer = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                writer.Write(typeAndMode);
                writer.Write(maxDev);
                writer.Write(durDays);
                writer.Write(mHash);

                writer.Write((byte)custBytes.Length);
                if (custBytes.Length > 0) writer.Write(custBytes);

                writer.Write((byte)compBytes.Length);
                if (compBytes.Length > 0) writer.Write(compBytes);
            }

            byte[] rawPayload = ms.ToArray();
            byte[] compressedPayload = CompressBytes(rawPayload);

            bool isCompressed = compressedPayload.Length < rawPayload.Length;
            byte[] payloadToUse = isCompressed ? compressedPayload : rawPayload;

            byte header = (byte)(isCompressed ? 0x80 : 0x00);

            using var hmac = new HMACSHA256(HmacSecretKey);
            byte[] fullHash = hmac.ComputeHash(payloadToUse);

            // 1 byte header + payload + 4 bytes truncated HMAC signature (32-bit security)
            byte[] fullBuffer = new byte[1 + payloadToUse.Length + 4];
            fullBuffer[0] = header;
            Array.Copy(payloadToUse, 0, fullBuffer, 1, payloadToUse.Length);
            Array.Copy(fullHash, 0, fullBuffer, 1 + payloadToUse.Length, 4);

            string b32 = ToBase32(fullBuffer);

            var sb = new StringBuilder("RA-");
            for (int i = 0; i < b32.Length; i++)
            {
                if (i > 0 && i % 5 == 0) sb.Append('-');
                sb.Append(b32[i]);
            }
            return sb.ToString();
        }

        public static (bool Success, LicenseData? Data, string Error) DecodeCompactCode(string code, string currentMachineId)
        {
            if (string.IsNullOrWhiteSpace(code)) 
                return (false, null, "كود التفعيل فارغ.");

            string clean = code.Trim().ToUpperInvariant();
            if (clean.StartsWith("RA-")) clean = clean.Substring(3);
            clean = clean.Replace("-", "").Replace(" ", "");

            byte[] fullBuffer;
            try
            {
                fullBuffer = FromBase32(clean);
            }
            catch
            {
                return (false, null, "تنسيق كود التفعيل غير صالح.");
            }

            if (fullBuffer.Length < 10) 
                return (false, null, "كود التفعيل ناقص أو غير مكتمل.");

            byte header = fullBuffer[0];
            bool isCompressed = (header & 0x80) != 0;

            int payloadLength = fullBuffer.Length - 1 - 4;
            byte[] payloadToUse = new byte[payloadLength];
            Array.Copy(fullBuffer, 1, payloadToUse, 0, payloadLength);

            using var hmac = new HMACSHA256(HmacSecretKey);
            byte[] expectedHash = hmac.ComputeHash(payloadToUse);

            int sigStartIndex = 1 + payloadLength;
            for (int i = 0; i < 4; i++)
            {
                if (fullBuffer[sigStartIndex + i] != expectedHash[i])
                {
                    return (false, null, "كود التفعيل غير صحيح أو تم التلاعب بمحتواه (التوقيع غير مطابق).");
                }
            }

            byte[] rawPayload = isCompressed ? DecompressBytes(payloadToUse) : payloadToUse;

            try
            {
                using var ms = new MemoryStream(rawPayload);
                using var reader = new BinaryReader(ms, Encoding.UTF8);

                byte typeAndMode = reader.ReadByte();
                ushort maxDev = reader.ReadUInt16();
                ushort durDays = reader.ReadUInt16();
                uint mHash = reader.ReadUInt32();

                string customerName = "عميل الرافدين";
                string companyName = "الرافدين ERP";

                if (ms.Position < ms.Length)
                {
                    byte custLen = reader.ReadByte();
                    if (custLen > 0 && ms.Position + custLen <= ms.Length)
                    {
                        byte[] cBytes = reader.ReadBytes(custLen);
                        customerName = Encoding.UTF8.GetString(cBytes);
                    }
                }

                if (ms.Position < ms.Length)
                {
                    byte compLen = reader.ReadByte();
                    if (compLen > 0 && ms.Position + compLen <= ms.Length)
                    {
                        byte[] coBytes = reader.ReadBytes(compLen);
                        companyName = Encoding.UTF8.GetString(coBytes);
                    }
                }

                var subType = (LicenseType)(typeAndMode & 0x0F);
                var mode = (ActivationMode)((typeAndMode >> 4) & 0x0F);

                if (mHash != 0)
                {
                    uint currentHash = ComputeCrc32(currentMachineId.Trim().ToUpperInvariant());
                    if (mHash != currentHash)
                    {
                        return (false, null, "كود التفعيل مخصص لجهاز آخر ولا يطابق كود هذا الجهاز.");
                    }
                }

                int maxDevices = maxDev == 0xFFFF ? -1 : maxDev;
                int days = durDays > 0 ? durDays : 30;

                DateTime now = DateTime.Now;
                DateTime expDate = subType == LicenseType.Lifetime 
                    ? now.AddYears(100) 
                    : now.AddDays(days);

                var data = new LicenseData
                {
                    LicenseKey = code,
                    SubscriptionType = subType,
                    Mode = mode,
                    MaxDevices = maxDevices,
                    DurationDays = days,
                    MachineId = mHash == 0 ? "*" : currentMachineId,
                    ActivationDate = now,
                    IssueDate = now,
                    ExpirationDate = expDate,
                    Status = LicenseStatus.Active,
                    CustomerName = string.IsNullOrWhiteSpace(customerName) ? "عميل الرافدين" : customerName,
                    CompanyName = string.IsNullOrWhiteSpace(companyName) ? "الرافدين ERP" : companyName
                };

                return (true, data, string.Empty);
            }
            catch
            {
                return (false, null, "حدث خطأ أثناء قراءة بيانات الكود.");
            }
        }

        private static byte[] CompressBytes(byte[] data)
        {
            using var outputStream = new MemoryStream();
            using (var deflateStream = new DeflateStream(outputStream, CompressionLevel.Optimal))
            {
                deflateStream.Write(data, 0, data.Length);
            }
            return outputStream.ToArray();
        }

        private static byte[] DecompressBytes(byte[] compressedData)
        {
            using var inputStream = new MemoryStream(compressedData);
            using var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress);
            using var outputStream = new MemoryStream();
            deflateStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }

        private static uint ComputeCrc32(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            uint crc = 0xFFFFFFFF;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                crc ^= b;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
            }
            return ~crc;
        }

        public static string ToBase32(byte[] data)
        {
            if (data == null || data.Length == 0) return string.Empty;
            var result = new StringBuilder((data.Length * 8 + 4) / 5);
            int buffer = data[0];
            int next = 1;
            int bitsLeft = 8;

            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < 5)
                {
                    if (next < data.Length)
                    {
                        buffer = (buffer << 8) | (data[next++] & 0xFF);
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = 5 - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }

                int index = (buffer >> (bitsLeft - 5)) & 0x1F;
                bitsLeft -= 5;
                result.Append(Base32Alphabet[index]);
            }

            return result.ToString();
        }

        public static byte[] FromBase32(string base32)
        {
            if (string.IsNullOrEmpty(base32)) return Array.Empty<byte>();

            string clean = base32.Trim().ToUpperInvariant();
            var bytes = new List<byte>();
            int buffer = 0;
            int bitsLeft = 0;

            foreach (char c in clean)
            {
                int val = Base32Alphabet.IndexOf(c);
                if (val < 0) continue;

                buffer = (buffer << 5) | val;
                bitsLeft += 5;

                if (bitsLeft >= 8)
                {
                    bitsLeft -= 8;
                    bytes.Add((byte)((buffer >> bitsLeft) & 0xFF));
                    buffer &= (1 << bitsLeft) - 1;
                }
            }

            return bytes.ToArray();
        }
    }
}
