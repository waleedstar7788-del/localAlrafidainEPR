using CommunityToolkit.Mvvm.ComponentModel;

namespace RetailApp.ViewModels
{
    public partial class PlaceholderViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _pageTitle = "تحت الإنشاء";

        [ObservableProperty]
        private string _iconKind = "HammerWrench";

        public void Initialize(string title, string icon)
        {
            PageTitle = title;
            IconKind = icon;
        }
    }
}
