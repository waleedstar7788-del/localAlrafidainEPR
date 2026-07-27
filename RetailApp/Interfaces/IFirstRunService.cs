using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IFirstRunService
    {
        Task<bool> IsFirstRunRequiredAsync();
    }
}
