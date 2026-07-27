using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class ThemeService : IThemeService
    {
        private readonly ISettingsService _settingsService;
        private readonly PaletteHelper _paletteHelper;

        public ThemeService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _paletteHelper = new PaletteHelper();
        }

        public void SwitchTheme(string themeMode)
        {
            ITheme theme = _paletteHelper.GetTheme();
            
            bool isDark = themeMode == "Dark" || (themeMode == "System" && SystemParameters.HighContrast);
            
            IBaseTheme baseTheme = isDark ? (IBaseTheme)new MaterialDesignDarkTheme() : new MaterialDesignLightTheme();

            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
            
            // Switch custom theme dictionaries
            if (Application.Current != null)
            {
                var appDictionaries = Application.Current.Resources.MergedDictionaries;
                var themeDict = new ResourceDictionary();
                
                string themeUri = isDark 
                    ? "pack://application:,,,/RetailApp;component/Styles/Themes/DarkTheme.xaml"
                    : "pack://application:,,,/RetailApp;component/Styles/Themes/LightTheme.xaml";
                    
                themeDict.Source = new System.Uri(themeUri);
                
                // Remove old theme
                for (int i = appDictionaries.Count - 1; i >= 0; i--)
                {
                    if (appDictionaries[i].Source != null && appDictionaries[i].Source.ToString().Contains("Theme.xaml") && !appDictionaries[i].Source.ToString().Contains("MaterialDesignTheme"))
                    {
                        appDictionaries.RemoveAt(i);
                    }
                }
                
                appDictionaries.Add(themeDict);
            }
        }

        public void SetAccentColor(string colorHex)
        {
            try
            {
                ITheme theme = _paletteHelper.GetTheme();
                var color = (Color)ColorConverter.ConvertFromString(colorHex);
                
                theme.SetPrimaryColor(color);
                theme.SetSecondaryColor(color);
                
                _paletteHelper.SetTheme(theme);
            }
            catch
            {
                // Invalid color hex, do nothing
            }
        }

        public async Task ApplyInitialThemeAsync()
        {
            var localSettings = await _settingsService.GetLocalSettingsAsync();
            SwitchTheme(localSettings.Theme);
            
            if (!string.IsNullOrEmpty(localSettings.AccentColor) && localSettings.AccentColor.StartsWith("#"))
            {
                SetAccentColor(localSettings.AccentColor);
            }
        }
    }
}
