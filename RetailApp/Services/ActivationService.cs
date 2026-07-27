using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class ActivationService : IActivationService
    {
        private readonly ILicenseValidationService _validationService;
        private readonly ILicenseService _licenseService;
        private readonly IMachineIdService _machineIdService;

        public event Action? OnLicenseChanged;

        public ActivationService(
            ILicenseValidationService validationService, 
            ILicenseService licenseService,
            IMachineIdService machineIdService)
        {
            _validationService = validationService;
            _licenseService = licenseService;
            _machineIdService = machineIdService;
        }

        public async Task<(bool Success, string Message)> ActivateOfflineAsync(string licenseInput)
        {
            if (string.IsNullOrWhiteSpace(licenseInput))
                return (false, "يرجى إدخال كود التفعيل النصي (RA-...).");

            try
            {
                string currentMachineId = _machineIdService.GetMachineId();

                var compactRes = CompactKeyService.DecodeCompactCode(licenseInput, currentMachineId);
                if (compactRes.Success && compactRes.Data != null)
                {
                    await _licenseService.SaveLicenseAsync(compactRes.Data);
                    OnLicenseChanged?.Invoke();

                    string subName = compactRes.Data.SubscriptionType switch
                    {
                        LicenseType.Lifetime => "مدى الحياة ♾️",
                        LicenseType.Yearly => "سنوي (365 يوم) 📅",
                        LicenseType.Monthly => "شهري (30 يوم) 🗓️",
                        LicenseType.Quarterly => "ثلاثي الأشهر (90 يوم) 🗓️",
                        LicenseType.Trial => "تجريبي (14 يوم) ⏳",
                        _ => "مخصص"
                    };
                    return (true, $"تم تفعيل البرنامج بنجاح! نوع الاشتراك: {subName}");
                }

                if (!string.IsNullOrWhiteSpace(compactRes.Error))
                {
                    return (false, compactRes.Error);
                }

                return (false, "كود التفعيل غير صالح. يرجى التأكد من كتابة الكود بشكل صحيح.");
            }
            catch (Exception ex)
            {
                return (false, $"حدث خطأ أثناء فحص كود التفعيل: {ex.Message}");
            }
        }

        public async Task<bool> DeactivateAsync()
        {
            await _licenseService.ClearLicenseAsync();
            OnLicenseChanged?.Invoke();
            return true;
        }

        public Task<string> ExportMachineRequestAsync()
        {
            var req = new 
            {
                MachineId = _machineIdService.GetMachineId(),
                RequestDate = DateTime.Now
            };
            return Task.FromResult(JsonSerializer.Serialize(req));
        }
    }
}
