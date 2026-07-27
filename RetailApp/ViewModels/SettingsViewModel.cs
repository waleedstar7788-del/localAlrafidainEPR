using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IThemeService _themeService;

        public BackupViewModel BackupViewModel { get; }
        public LicenseViewModel LicenseViewModel { get; }
        public PrintDesignerViewModel PrintDesignerViewModel { get; }
        public UpdateViewModel UpdateViewModel { get; }

        [ObservableProperty]
        private AppSettings _dbSettings;

        [ObservableProperty]
        private LocalSettings _localSettings;

        [ObservableProperty]
        private ObservableCollection<string> _categories;

        [ObservableProperty]
        private string _selectedCategory;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _isSaving;

        public ICommand SaveSettingsCommand { get; }
        public ICommand ApplyThemeCommand { get; }
        public ICommand ApplyAccentColorCommand { get; }

        public SettingsViewModel(ISettingsService settingsService, IThemeService themeService, BackupViewModel backupViewModel, LicenseViewModel licenseViewModel, PrintDesignerViewModel printDesignerViewModel, UpdateViewModel updateViewModel)
        {
            _settingsService = settingsService;
            _themeService = themeService;
            BackupViewModel = backupViewModel;
            LicenseViewModel = licenseViewModel;
            PrintDesignerViewModel = printDesignerViewModel;
            UpdateViewModel = updateViewModel;

            _dbSettings = new AppSettings();
            _localSettings = new LocalSettings();
            _statusMessage = string.Empty;
            _selectedCategory = "إعدادات الشركة";

            Categories = new ObservableCollection<string>
            {
                "إعدادات الشركة",
                "الإعدادات العامة",
                "نقطة البيع (POS)",
                "الطباعة",
                "المخزون",
                "الحسابات",
                "الأمان",
                "الإشعارات",
                "المظهر والثيمات",
                "النسخ الاحتياطي",
                "تحديثات النظام",
                "تفعيل الترخيص",
                "حول البرنامج"
            };

            SelectedCategory = Categories[0];

            SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);
            ApplyThemeCommand = new RelayCommand<string>(ApplyTheme);
            ApplyAccentColorCommand = new RelayCommand<string>(ApplyAccentColor);

            LoadSettingsAsync();
        }

        private async void LoadSettingsAsync()
        {
            try
            {
                DbSettings = await _settingsService.GetDbSettingsAsync();
                LocalSettings = await _settingsService.GetLocalSettingsAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"خطأ في تحميل الإعدادات: {ex.Message}";
                if (DbSettings == null) DbSettings = new AppSettings();
                if (LocalSettings == null) LocalSettings = new LocalSettings();
            }
        }

        private async Task SaveSettingsAsync()
        {
            IsSaving = true;
            StatusMessage = "جاري الحفظ...";
            
            try
            {
                await _settingsService.SaveDbSettingsAsync(DbSettings);
                await _settingsService.SaveLocalSettingsAsync(LocalSettings);
                StatusMessage = "تم حفظ الإعدادات بنجاح.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"حدث خطأ أثناء الحفظ: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
                await Task.Delay(3000);
                StatusMessage = string.Empty;
            }
        }

        private void ApplyTheme(string? theme)
        {
            if (string.IsNullOrEmpty(theme)) return;
            LocalSettings.Theme = theme;
            _themeService.SwitchTheme(theme);
        }

        private void ApplyAccentColor(string? colorHex)
        {
            if (string.IsNullOrEmpty(colorHex)) return;
            LocalSettings.AccentColor = colorHex;
            _themeService.SetAccentColor(colorHex);
        }
    }
}
