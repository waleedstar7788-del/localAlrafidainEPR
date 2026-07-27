using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using LicenseGenerator.Models;
using LicenseGenerator.Services;

namespace LicenseGenerator
{
    public partial class MainWindow : Window
    {
        private string? _previousMachineId = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnUniversalMachine_Click(object sender, RoutedEventArgs e)
        {
            if (TxtMachineId == null) return;
            var currentText = TxtMachineId.Text.Trim();

            if (currentText == "*")
            {
                TxtMachineId.Text = _previousMachineId ?? "";
            }
            else
            {
                _previousMachineId = currentText;
                TxtMachineId.Text = "*";
            }
        }

        private void TxtMachineId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BtnUniversalMachine == null || TxtMachineId == null) return;

            if (TxtMachineId.Text.Trim() == "*")
            {
                BtnUniversalMachine.Content = "إعادة كود الجهاز السابق ↩";
                BtnUniversalMachine.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#059669"));
            }
            else
            {
                BtnUniversalMachine.Content = "ترخيص عام لجميع الأجهزة (*)";
                BtnUniversalMachine.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB"));
            }
        }

        private void CmbLicenseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int days = 365;
            switch (CmbLicenseType.SelectedIndex)
            {
                case 0: // Trial
                    days = 14;
                    break;
                case 1: // Monthly
                    days = 30;
                    break;
                case 2: // Quarterly
                    days = 90;
                    break;
                case 3: // Yearly
                    days = 365;
                    break;
                case 4: // Lifetime
                    days = 36500;
                    break;
                case 5: // Custom
                    days = 30;
                    break;
            }

            if (TxtDurationDays != null)
            {
                TxtDurationDays.Text = days.ToString();
            }
        }

        private LicenseData BuildLicenseDataFromUI()
        {
            LicenseType selectedType = LicenseType.Yearly;
            int defaultDays = 365;
            switch (CmbLicenseType.SelectedIndex)
            {
                case 0: selectedType = LicenseType.Trial; defaultDays = 14; break;
                case 1: selectedType = LicenseType.Monthly; defaultDays = 30; break;
                case 2: selectedType = LicenseType.Quarterly; defaultDays = 90; break;
                case 3: selectedType = LicenseType.Yearly; defaultDays = 365; break;
                case 4: selectedType = LicenseType.Lifetime; defaultDays = 36500; break;
                case 5: selectedType = LicenseType.Custom; defaultDays = 30; break;
            }

            int maxDevices = 1;
            string rawMaxText = CmbMaxDevices.Text?.Trim() ?? "1";
            if (rawMaxText.Contains("*") || rawMaxText.Contains("غير محدود"))
            {
                maxDevices = -1;
            }
            else
            {
                string digits = System.Text.RegularExpressions.Regex.Match(rawMaxText, @"\d+").Value;
                if (int.TryParse(digits, out int parsedMax) && parsedMax > 0)
                {
                    maxDevices = parsedMax;
                }
            }

            int durationDays = defaultDays;
            if (int.TryParse(TxtDurationDays?.Text?.Trim(), out int days) && days > 0)
            {
                durationDays = days;
            }

            string machineId = TxtMachineId.Text.Trim();
            if (string.IsNullOrWhiteSpace(machineId)) machineId = "*";

            string custName = TxtCustomerName.Text.Trim();
            if (string.IsNullOrWhiteSpace(custName)) custName = "عميل الرافدين";

            string compName = TxtCompanyName.Text.Trim();
            if (string.IsNullOrWhiteSpace(compName)) compName = "الرافدين ERP";

            ActivationMode mode = CmbActivationMode.SelectedIndex == 0
                ? ActivationMode.FromFirstUse
                : ActivationMode.FixedDates;

            return new LicenseData
            {
                CustomerName = custName,
                CompanyName = compName,
                MachineId = machineId,
                SubscriptionType = selectedType,
                MaxDevices = maxDevices,
                Mode = mode,
                DurationDays = durationDays > 0 ? durationDays : 30,
                IssueDate = DateTime.Now,
                ExpirationDate = selectedType == LicenseType.Lifetime ? DateTime.Now.AddYears(100) : DateTime.Now.AddDays(durationDays)
            };
        }

        private void BtnGenerateLicenseCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var license = BuildLicenseDataFromUI();
                string code = CompactKeyService.EncodeCompactCode(license);

                TxtGeneratedCode.Text = code;
                Clipboard.SetText(code);

                string targetMachine = license.MachineId == "*" ? "جميع الأجهزة (ترخيص عام *)" : license.MachineId;
                string devicesText = license.MaxDevices <= 0 ? "غير محدود" : license.MaxDevices.ToString();
                string modeText = license.Mode == ActivationMode.FromFirstUse
                    ? "من بداية الاستخدام عند إدخال الكود ⚡"
                    : "من تاريخ إنشاء الكود اليوم 📅";

                string subTypeName = license.SubscriptionType switch
                {
                    LicenseType.Lifetime => "مدى الحياة (Lifetime ♾️)",
                    LicenseType.Yearly => "سنوي (Yearly 📅)",
                    LicenseType.Monthly => "شهري (Monthly 🗓️)",
                    LicenseType.Quarterly => "ثلاثي الأشهر (90 يوم)",
                    LicenseType.Trial => "تجريبي (Trial ⏳)",
                    _ => "مخصص"
                };

                TxtPayloadDetails.Text = 
                    $"• اسم العميل المحقون: {license.CustomerName}\n" +
                    $"• اسم الشركة المحقونة: {license.CompanyName}\n" +
                    $"• نمط التفعيل: {modeText}\n" +
                    $"• نوع الاشتراك: {subTypeName}\n" +
                    $"• مدة الفعالية: {license.DurationDays} يوم\n" +
                    $"• الجهاز المستهدف: {targetMachine}\n" +
                    $"• الأجهزة المسموح بها: {devicesText}\n" +
                    $"• حالة التوقيع الأمني: موقع مشفر بـ HMAC-SHA256 مضاد للتلاعب ✔";

                TxtStatusLog.Text = $"تم توليد وحقن كود التفعيل بنجاح ونسخه للحافظة: {code}";
                MessageBox.Show($"تم إنشاء كود التفعيل النصي المرمز وحقن (نمط التفعيل والبيانات) وتوقيعه بنجاح!\n\nتم نسخ الكود تلقائياً إلى الحافظة (Clipboard).\n\nالكود:\n{code}", "تم إنشاء الكود بنجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء إنشاء كود التفعيل: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCopyCode_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtGeneratedCode.Text))
            {
                Clipboard.SetText(TxtGeneratedCode.Text);
                MessageBox.Show("تم نسخ كود التفعيل إلى الحافظة بنجاح!", "تم النسخ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("يرجى الضغط على زر إنشاء كود التفعيل أولاً.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
