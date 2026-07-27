using System.Reflection;
using RetailApp.Interfaces;

namespace RetailApp.Services
{
    public class VersionService : IVersionService
    {
        public string GetCurrentVersion()
        {
            return "1.0.3";
        }

        public string GetBuildNumber()
        {
            return "2026.07.27";
        }

        public string GetReleaseDate()
        {
            return "2026-07-27";
        }
    }
}
