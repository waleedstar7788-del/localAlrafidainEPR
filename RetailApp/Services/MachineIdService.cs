using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class MachineIdService : IMachineIdService
    {
        public string GetMachineId()
        {
            string machineGuid = GetWindowsMachineGuid();
            
            // Fallback if MachineGuid registry key is missing
            if (string.IsNullOrEmpty(machineGuid))
            {
                machineGuid = Environment.MachineName;
            }

            var rawId = $"RETAIL-HW-{machineGuid}";
            
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawId));
            var hash = BitConverter.ToString(bytes).Replace("-", "").Substring(0, 16);
            
            return $"RET-{hash}";
        }

        private static string GetWindowsMachineGuid()
        {
            try
            {
                using var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                return key?.GetValue("MachineGuid")?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
