using RetailApp.Models;
using System.Threading.Tasks;

namespace RetailApp.Interfaces
{
    public interface IPrintService
    {
        Task<bool> PrintDocumentAsync(object dataContext, PrintTemplate template, string printerName = null, bool showPrintDialog = false);
    }
}
