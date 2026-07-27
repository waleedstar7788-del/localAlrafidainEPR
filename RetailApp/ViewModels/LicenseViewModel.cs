using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.ViewModels
{
    public partial class LicenseViewModel : BaseViewModel
    {
        private readonly ILicenseService _licenseService;
        private readonly IActivationService _activationService;
        private readonly IMachineIdService _machineIdService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private LicenseData? _currentLicense;

        [ObservableProperty]
        private string _machineId = string.Empty;

        [ObservableProperty]
        private string _licenseKeyInput = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isActivating;

        [ObservableProperty]
        private bool _isActivated;

        [ObservableProperty]
        private bool _isSuccessMessage;

        [ObservableProperty]
        private string _licenseRemainingText = string.Empty;

        public ICommand ActivateCommand { get; }
        public ICommand CopyMachineIdCommand { get; }
        public ICommand ContinueToAppCommand { get; }
        public ICommand DeactivateLicenseCommand { get; }

        public LicenseViewModel(
            ILicenseService licenseService,
            IActivationService activationService,
            IMachineIdService machineIdService,
            INavigationService navigationService)
        {
            _licenseService = licenseService;
            _activationService = activationService;
            _machineIdService = machineIdService;
            _navigationService = navigationService;

            ActivateCommand = new AsyncRelayCommand(ActivateAsync);
            CopyMachineIdCommand = new RelayCommand(() => System.Windows.Clipboard.SetText(MachineId));
            ContinueToAppCommand = new RelayCommand(ContinueToApp);
            DeactivateLicenseCommand = new AsyncRelayCommand(DeactivateLicenseAsync);

            LoadLicenseDataAsync();
        }

        private async void LoadLicenseDataAsync()
        {
            MachineId = _machineIdService.GetMachineId();
            CurrentLicense = await _licenseService.GetCurrentLicenseAsync();
            var status = await _licenseService.ValidateCurrentLicenseAsync();
            IsActivated = (status == LicenseStatus.Active || status == LicenseStatus.Trial);

            UpdateRemainingText();
        }

        private void UpdateRemainingText()
        {
            if (!IsActivated || CurrentLicense == null)
            {
                LicenseRemainingText = "الاشتراك غير مفعل ⚠️";
            }
            else if (CurrentLicense.SubscriptionType == LicenseType.Lifetime)
            {
                LicenseRemainingText = "الاشتراك: مدى الحياة ♾️";
            }
            else
            {
                LicenseRemainingText = $"متبقي على الاشتراك: {CurrentLicense.RemainingDays} يوم ⏳";
            }
        }

        private async Task DeactivateLicenseAsync()
        {
            IsActivating = true;
            await _activationService.DeactivateAsync();
            CurrentLicense = null;
            IsActivated = false;
            StatusMessage = "تم إنهاء التفعيل الحالي بنجاح. يمكنك الآن أدخال كود ترخيص جديد لتفعيل التطبيق.";
            IsSuccessMessage = true;
            UpdateRemainingText();
            IsActivating = false;
        }

        private async Task ActivateAsync()
        {
            if (string.IsNullOrWhiteSpace(LicenseKeyInput))
            {
                StatusMessage = "يرجى إدخال أو لصق كود التفعيل المستلم (RA-...).";
                IsSuccessMessage = false;
                return;
            }

            IsActivating = true;
            StatusMessage = "جاري التحقق من كود التفعيل وفك التشفير...";
            IsSuccessMessage = false;

            await Task.Delay(500);

            var (success, message) = await _activationService.ActivateOfflineAsync(LicenseKeyInput);

            IsActivating = false;
            StatusMessage = message;
            IsSuccessMessage = success;

            if (success)
            {
                CurrentLicense = await _licenseService.GetCurrentLicenseAsync();
                IsActivated = true;
                UpdateRemainingText();
            }
        }

        private void ContinueToApp()
        {
            _navigationService.NavigateTo<LoginViewModel>();
        }
    }
}
