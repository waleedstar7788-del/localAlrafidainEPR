namespace RetailApp.Interfaces
{
    public interface IEnvironmentCheckService
    {
        bool ValidateEnvironment(out string errorMessage);
    }
}
