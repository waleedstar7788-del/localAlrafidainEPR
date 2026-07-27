using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IThemeService
    {
        void SwitchTheme(string themeMode);
        void SetAccentColor(string colorHex);
        Task ApplyInitialThemeAsync();
    }
}
