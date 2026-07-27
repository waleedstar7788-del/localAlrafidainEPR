using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetailApp.Services
{
    public class PrintTemplateManager : IPrintTemplateManager
    {
        private readonly string _templatesDirectory;

        public PrintTemplateManager()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _templatesDirectory = Path.Combine(appData, "RetailApp", "PrintTemplates");
            if (!Directory.Exists(_templatesDirectory))
            {
                Directory.CreateDirectory(_templatesDirectory);
                CreateDefaultTemplatesAsync().Wait();
            }
        }

        private async Task CreateDefaultTemplatesAsync()
        {
            var defaultThermal = new PrintTemplate
            {
                Name = "إيصال حراري افتراضي (80mm)",
                DocumentType = "SalesInvoice",
                PaperSize = "80mm",
                ReceiptWidth = 280,
                FontFamily = "Cairo",
                FontSize = 12,
                PrimaryColor = "#000000",
                SecondaryColor = "#444444"
            };
            
            var defaultA4 = new PrintTemplate
            {
                Name = "فاتورة ضريبية افتراضية (A4)",
                DocumentType = "SalesInvoice",
                PaperSize = "A4",
                Margins = "30,30,30,30",
                FontFamily = "Cairo",
                FontSize = 14,
                PrimaryColor = "#1e3a8a", // Blue-900
                SecondaryColor = "#475569", // Slate-600
                ShowItemDiscount = true
            };

            await SaveTemplateAsync(defaultThermal);
            await SaveTemplateAsync(defaultA4);
        }

        public async Task<List<PrintTemplate>> GetAllTemplatesAsync()
        {
            var templates = new List<PrintTemplate>();
            var files = Directory.GetFiles(_templatesDirectory, "*.json");
            
            foreach (var file in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var template = JsonSerializer.Deserialize<PrintTemplate>(json);
                    if (template != null)
                    {
                        templates.Add(template);
                    }
                }
                catch
                {
                    // Ignore corrupted files
                }
            }
            
            return templates.OrderBy(t => t.Name).ToList();
        }

        public async Task<PrintTemplate> GetTemplateByIdAsync(string id)
        {
            var filePath = Path.Combine(_templatesDirectory, $"{id}.json");
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<PrintTemplate>(json) ?? new PrintTemplate();
            }
            return null;
        }

        public async Task<PrintTemplate> GetDefaultTemplateAsync(string documentType, string paperSize)
        {
            var all = await GetAllTemplatesAsync();
            return all.FirstOrDefault(t => t.DocumentType == documentType && t.PaperSize == paperSize) 
                   ?? all.FirstOrDefault() 
                   ?? new PrintTemplate();
        }

        public async Task SaveTemplateAsync(PrintTemplate template)
        {
            var filePath = Path.Combine(_templatesDirectory, $"{template.Id}.json");
            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task DeleteTemplateAsync(string id)
        {
            var filePath = Path.Combine(_templatesDirectory, $"{id}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await Task.CompletedTask;
        }

        public async Task<PrintTemplate> CreateNewTemplateAsync(string documentType, string paperSize, string name)
        {
            var template = new PrintTemplate
            {
                Name = name,
                DocumentType = documentType,
                PaperSize = paperSize
            };
            
            await SaveTemplateAsync(template);
            return template;
        }
    }
}
