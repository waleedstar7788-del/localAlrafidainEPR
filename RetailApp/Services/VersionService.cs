using System.Reflection;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class VersionService : IVersionService
    {
        public string GetCurrentVersion()
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (assemblyVersion != null)
            {
                return $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
            }
            return "1.0.0";
        }

        public string GetBuildNumber()
        {
            return "2026.07.19";
        }

        public string GetReleaseDate()
        {
            return "2026-07-19";
        }
    }
}
