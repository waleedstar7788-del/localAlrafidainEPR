using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailApp.Interfaces;
using RetailApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Win32;

namespace RetailApp.ViewModels
{
    public partial class PrintDesignerViewModel : BaseViewModel
    {
        private readonly IPrintTemplateManager _templateManager;
        private readonly INotificationService _notificationService;
        private readonly IDialogService _dialogService;
        private readonly IPrintService _printService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private ObservableCollection<PrintTemplate> _templates = new();

        [ObservableProperty]
        private PrintTemplate _activeTemplate;

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        [ObservableProperty]
        private PrintContext _previewContext;

        public string ZoomPercentage => $"{(int)(ZoomLevel * 100)}%";

        public PrintDesignerViewModel(
            IPrintTemplateManager templateManager,
            INotificationService notificationService,
            IDialogService dialogService,
            IPrintService printService,
            ISettingsService settingsService)
        {
            _templateManager = templateManager;
            _notificationService = notificationService;
            _dialogService = dialogService;
            _printService = printService;
            _settingsService = settingsService;

            LoadDataAsync().ConfigureAwait(false);
        }

        private async Task LoadDataAsync()
        {
            var list = await _templateManager.GetAllTemplatesAsync();
            Templates.Clear();
            foreach (var t in list) Templates.Add(t);

            if (Templates.Any())
            {
                ActiveTemplate = Templates.First();
                UpdatePreviewContext();
            }
        }

        partial void OnActiveTemplateChanged(PrintTemplate value)
        {
            UpdatePreviewContext();
            ZoomLevel = 1.0; // Reset zoom on template change
        }

        partial void OnZoomLevelChanged(double value)
        {
            OnPropertyChanged(nameof(ZoomPercentage));
        }

        private void UpdatePreviewContext()
        {
            if (ActiveTemplate != null)
            {
                var settings = _settingsService.GetDbSettingsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                PreviewContext = new PrintContext
                {
                    Template = ActiveTemplate,
                    Invoice = CreateMockInvoice(),
                    GlobalSettings = settings
                };
            }
            else
            {
                PreviewContext = null;
            }
        }

        private SalesInvoice CreateMockInvoice()
        {
            // Creates a realistic mock invoice for the live preview
            var invoice = new SalesInvoice
            {
                Id = 1,
                InvoiceNumber = "INV-2026-0001",
                InvoiceDate = DateTime.Now,
                Customer = new Customer { FullName = "شركة التقنية المتقدمة للتجارة" },
                CashierName = "أحمد محمد (مدير المبيعات)",
                GrandTotal = 145000,
                SubTotal = 150000,
                DiscountAmount = 5000,
                Items = new ObservableCollection<SalesItem>
                {
                    new SalesItem { Id = 1, Product = new Product { Name = "لابتوب ديل XPS 15" }, Quantity = 1, UnitPrice = 120000, SubTotal = 120000 },
                    new SalesItem { Id = 2, Product = new Product { Name = "ماوس لاسلكي لوجيتك" }, Quantity = 2, UnitPrice = 10000, SubTotal = 20000 },
                    new SalesItem { Id = 3, Product = new Product { Name = "لوحة مفاتيح ميكانيكية" }, Quantity = 1, UnitPrice = 10000, SubTotal = 10000 }
                }
            };
            return invoice;
        }

        [RelayCommand]
        private void ZoomIn()
        {
            if (ZoomLevel < 2.5) ZoomLevel += 0.1;
        }

        [RelayCommand]
        private void ZoomOut()
        {
            if (ZoomLevel > 0.4) ZoomLevel -= 0.1;
        }

        [RelayCommand]
        private void ZoomReset()
        {
            ZoomLevel = 1.0;
        }

        [RelayCommand]
        private async Task SaveTemplateAsync()
        {
            if (ActiveTemplate != null)
            {
                await _templateManager.SaveTemplateAsync(ActiveTemplate);
                _notificationService.ShowSuccess("تم حفظ التعديلات بنجاح.");
            }
        }

        [RelayCommand]
        private async Task CreateNewTemplateAsync()
        {
            var newTemplate = await _templateManager.CreateNewTemplateAsync("SalesInvoice", "A4", "قالب جديد " + DateTime.Now.ToString("HH:mm"));
            Templates.Add(newTemplate);
            ActiveTemplate = newTemplate;
        }

        [RelayCommand]
        private async Task DuplicateTemplateAsync()
        {
            if (ActiveTemplate != null)
            {
                var clone = ActiveTemplate.Clone();
                clone.Id = Guid.NewGuid().ToString();
                clone.Name = ActiveTemplate.Name + " (نسخة)";
                await _templateManager.SaveTemplateAsync(clone);
                Templates.Add(clone);
                ActiveTemplate = clone;
                _notificationService.ShowSuccess("تم تكرار القالب بنجاح.");
            }
        }

        [RelayCommand]
        private async Task DeleteTemplateAsync()
        {
            if (ActiveTemplate != null && Templates.Count > 1)
            {
                var confirm = await _dialogService.ShowConfirmationAsync($"هل أنت متأكد من حذف القالب '{ActiveTemplate.Name}'؟");
                if (confirm)
                {
                    await _templateManager.DeleteTemplateAsync(ActiveTemplate.Id);
                    Templates.Remove(ActiveTemplate);
                    ActiveTemplate = Templates.First();
                    _notificationService.ShowSuccess("تم حذف القالب.");
                }
            }
            else
            {
                _notificationService.ShowWarning("لا يمكن حذف آخر قالب موجود.");
            }
        }

        [RelayCommand]
        private async Task TestPrintAsync()
        {
            if (ActiveTemplate != null && PreviewContext != null)
            {
                _notificationService.ShowInfo("جاري تجهيز الطباعة الاختبارية...");
                await _printService.PrintDocumentAsync(PreviewContext.Invoice, ActiveTemplate, null, true);
            }
        }

        [RelayCommand]
        private void SelectLogo()
        {
            if (ActiveTemplate != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                    Title = "اختيار شعار"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    ActiveTemplate.LogoPath = openFileDialog.FileName;
                }
            }
        }
    }
}
