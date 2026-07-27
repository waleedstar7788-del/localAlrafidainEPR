using System;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.ViewModels
{
    public partial class DeveloperDashboardViewModel : BaseViewModel
    {
        private readonly ILicenseValidationService _validationService;

        [ObservableProperty]
        private string _targetMachineId = string.Empty;

        [ObservableProperty]
        private string _customerName = string.Empty;

        [ObservableProperty]
        private LicenseType _selectedLicenseType = LicenseType.Yearly;

        [ObservableProperty]
        private string _generatedLicenseJson = string.Empty;

        public ICommand GenerateLicenseCommand { get; }

        public DeveloperDashboardViewModel(ILicenseValidationService validationService)
        {
            _validationService = validationService;
            GenerateLicenseCommand = new RelayCommand(GenerateLicense);
        }

        private void GenerateLicense()
        {
            if (string.IsNullOrWhiteSpace(TargetMachineId) || string.IsNullOrWhiteSpace(CustomerName)) return;

            var expirationDate = SelectedLicenseType switch
            {
                LicenseType.Monthly => DateTime.Now.AddMonths(1),
                LicenseType.Quarterly => DateTime.Now.AddMonths(3),
                LicenseType.Yearly => DateTime.Now.AddYears(1),
                LicenseType.Lifetime => DateTime.Now.AddYears(100),
                LicenseType.Trial => DateTime.Now.AddDays(14),
                _ => DateTime.Now.AddYears(1)
            };

            var license = new LicenseData
            {
                LicenseKey = _validationService.GenerateLicenseKey(),
                CustomerName = CustomerName,
                MachineId = TargetMachineId,
                ActivationDate = DateTime.Now,
                ExpirationDate = expirationDate,
                SubscriptionType = SelectedLicenseType,
                Status = LicenseStatus.Active
            };

            license.Signature = _validationService.GenerateSignature(license);

            GeneratedLicenseJson = JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
