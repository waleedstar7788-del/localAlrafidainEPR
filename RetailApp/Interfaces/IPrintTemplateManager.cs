using RetailApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPrintTemplateManager
    {
        Task<List<PrintTemplate>> GetAllTemplatesAsync();
        Task<PrintTemplate> GetTemplateByIdAsync(string id);
        Task<PrintTemplate> GetDefaultTemplateAsync(string documentType, string paperSize);
        Task SaveTemplateAsync(PrintTemplate template);
        Task DeleteTemplateAsync(string id);
        Task<PrintTemplate> CreateNewTemplateAsync(string documentType, string paperSize, string name);
    }
}
