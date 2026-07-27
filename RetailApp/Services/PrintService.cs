using RetailApp.Interfaces;
using RetailApp.Models;
using System;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RetailApp.Services
{
    public class PrintService : IPrintService
    {
        private readonly INotificationService _notificationService;

        public PrintService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> PrintDocumentAsync(object dataContext, PrintTemplate template, string printerName = null, bool showPrintDialog = false)
        {
            return await Task.Run(() =>
            {
                bool success = false;
                
                // Must be invoked on the UI thread for WPF Visual operations
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        PrintDialog printDialog = new PrintDialog();

                        if (!string.IsNullOrEmpty(printerName))
                        {
                            try
                            {
                                printDialog.PrintQueue = new LocalPrintServer().GetPrintQueue(printerName);
                            }
                            catch
                            {
                                _notificationService.ShowWarning($"لم يتم العثور على الطابعة '{printerName}'. سيتم استخدام الطابعة الافتراضية.");
                            }
                        }

                        if (showPrintDialog)
                        {
                            if (printDialog.ShowDialog() != true)
                            {
                                return; // User cancelled
                            }
                        }

                        // Determine Document Type and Load View
                        FrameworkElement printVisual = null;
                        
                        // We construct a dynamic container with the exact print queue size
                        var pageableSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                        if (template.PaperSize == "80mm" || template.PaperSize == "58mm")
                        {
                            // Load Thermal Receipt Control dynamically
                            var type = Type.GetType("RetailApp.Controls.PrintTemplates.ReceiptPreviewTemplate");
                            if (type != null)
                            {
                                printVisual = (FrameworkElement)Activator.CreateInstance(type);
                                printVisual.Width = template.ReceiptWidth;
                            }
                        }
                        else // A4, A5 etc.
                        {
                            var type = Type.GetType("RetailApp.Controls.PrintTemplates.A4PreviewTemplate");
                            if (type != null)
                            {
                                printVisual = (FrameworkElement)Activator.CreateInstance(type);
                                printVisual.Width = pageableSize.Width;
                                // Handle margins for A4
                                if (TryParseMargins(template.Margins, out Thickness margins))
                                {
                                    printVisual.Margin = margins;
                                }
                            }
                        }

                        if (printVisual == null)
                        {
                            _notificationService.ShowError("تعذر تحميل قالب الطباعة.");
                            return;
                        }

                        // We create a wrapper to hold the visual and the template data
                        printVisual.DataContext = new PrintContext { Invoice = dataContext, Template = template };

                        // Measure and Arrange
                        if (double.IsInfinity(pageableSize.Height))
                        {
                            // For receipt printers, height is infinite.
                            printVisual.Measure(new Size(printVisual.Width, double.PositiveInfinity));
                            printVisual.Arrange(new Rect(new Point(0, 0), printVisual.DesiredSize));
                        }
                        else
                        {
                            // For A4
                            printVisual.Measure(pageableSize);
                            printVisual.Arrange(new Rect(new Point(0, 0), pageableSize));
                        }
                        
                        printVisual.UpdateLayout();

                        printDialog.PrintVisual(printVisual, $"طباعة فاتورة - {template.Name}");
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        _notificationService.ShowError($"خطأ أثناء الطباعة: {ex.Message}");
                    }
                });

                return success;
            });
        }

        private bool TryParseMargins(string marginsStr, out Thickness thickness)
        {
            thickness = new Thickness(0);
            if (string.IsNullOrWhiteSpace(marginsStr)) return false;

            var parts = marginsStr.Split(',');
            if (parts.Length == 4 && 
                double.TryParse(parts[0], out double left) &&
                double.TryParse(parts[1], out double top) &&
                double.TryParse(parts[2], out double right) &&
                double.TryParse(parts[3], out double bottom))
            {
                thickness = new Thickness(left, top, right, bottom);
                return true;
            }
            return false;
        }
    }
}
