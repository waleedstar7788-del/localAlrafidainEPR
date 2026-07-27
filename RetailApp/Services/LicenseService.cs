using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly string _licenseFilePath;
        private readonly ILicenseValidationService _validationService;
        private readonly IMachineIdService _machineIdService;

        private LicenseData? _cachedLicense;
        private LicenseStatus? _cachedStatus;

        public LicenseService(ILicenseValidationService validationService, IMachineIdService machineIdService)
        {
            _validationService = validationService;
            _machineIdService = machineIdService;
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDir = Path.Combine(localAppData, "RetailApp", "Licensing");
            if (!Directory.Exists(appDir)) Directory.CreateDirectory(appDir);
            _licenseFilePath = Path.Combine(appDir, "license.dat");
        }

        public async Task<LicenseData?> GetCurrentLicenseAsync()
        {
            if (_cachedLicense != null) return _cachedLicense;
            if (!File.Exists(_licenseFilePath)) return null;

            try
            {
                var json = await File.ReadAllTextAsync(_licenseFilePath);
                _cachedLicense = JsonSerializer.Deserialize<LicenseData>(json);
                return _cachedLicense;
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveLicenseAsync(LicenseData license)
        {
            _cachedLicense = license;
            _cachedStatus = license.Status;
            var json = JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_licenseFilePath, json);
        }

        public async Task<LicenseStatus> ValidateCurrentLicenseAsync()
        {
            if (_cachedStatus.HasValue && _cachedLicense != null) return _cachedStatus.Value;

            var license = await GetCurrentLicenseAsync();
            if (license == null)
            {
                _cachedStatus = LicenseStatus.Pending;
                return LicenseStatus.Pending;
            }

            string currentMachineId = _machineIdService.GetMachineId();

            // Check Machine ID matching
            if (license.MachineId != currentMachineId && license.MachineId != "*")
            {
                _cachedStatus = LicenseStatus.MachineMismatch;
                return LicenseStatus.MachineMismatch;
            }

            // Check Expiration Date
            if (license.SubscriptionType != LicenseType.Lifetime && license.ExpirationDate < DateTime.Now)
            {
                license.Status = LicenseStatus.Expired;
                _cachedStatus = LicenseStatus.Expired;
                await SaveLicenseAsync(license);
                return LicenseStatus.Expired;
            }

            // License is valid and within subscription period!
            if (license.Status != LicenseStatus.Active && license.Status != LicenseStatus.Trial)
            {
                license.Status = LicenseStatus.Active;
                await SaveLicenseAsync(license);
            }

            _cachedStatus = license.Status;
            return license.Status;
        }

        public async Task StartTrialAsync()
        {
            var trialData = new LicenseData
            {
                LicenseKey = "TRIAL-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                CustomerName = "عميل تجريبي",
                CompanyName = "الرافدين ERP",
                ActivationDate = DateTime.Now,
                IssueDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddDays(14),
                MachineId = _machineIdService.GetMachineId(),
                SubscriptionType = LicenseType.Trial,
                Status = LicenseStatus.Active
            };
            await SaveLicenseAsync(trialData);
        }

        public Task ClearLicenseAsync()
        {
            _cachedLicense = null;
            _cachedStatus = null;
            if (File.Exists(_licenseFilePath))
            {
                try { File.Delete(_licenseFilePath); } catch { }
            }
            return Task.CompletedTask;
        }
    }
}
