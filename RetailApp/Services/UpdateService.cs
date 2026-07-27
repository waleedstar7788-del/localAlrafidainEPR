using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using RetailApp.Interfaces;
using RetailApp.Models;

namespace RetailApp.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IVersionService _versionService;
        private readonly HttpClient _httpClient;

        // حساب ومستودع GitHub الخاص بالتطبيقات والتحديثات
        public string GitHubOwner { get; set; } = "waleedstar7788-del";
        public string GitHubRepo { get; set; } = "localAlrafidainEPR";

        public UpdateService(IVersionService versionService)
        {
            _versionService = versionService;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RetailApp-Updater", "1.0"));
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                // 1. محاولة الفحص من خلال REST API الرسمي لـ GitHub
                string apiUrl = $"https://api.github.com/repos/{GitHubOwner}/{GitHubRepo}/releases/latest";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    string tagName = root.TryGetProperty("tag_name", out var tagElem) ? tagElem.GetString() ?? "" : "";
                    string cleanVersion = tagName.TrimStart('v', 'V');
                    string releaseNotes = root.TryGetProperty("body", out var bodyElem) ? bodyElem.GetString() ?? "" : "";

                    DateTime releaseDate = DateTime.Now;
                    if (root.TryGetProperty("published_at", out var pubElem) && pubElem.TryGetDateTime(out var parsedDate))
                    {
                        releaseDate = parsedDate;
                    }

                    string downloadUrl = "";
                    if (root.TryGetProperty("assets", out var assetsElem) && assetsElem.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var asset in assetsElem.EnumerateArray())
                        {
                            string dl = asset.TryGetProperty("browser_download_url", out var dlElem) ? dlElem.GetString() ?? "" : "";
                            if (!string.IsNullOrEmpty(dl))
                            {
                                downloadUrl = dl;
                                string name = asset.TryGetProperty("name", out var nameElem) ? nameElem.GetString() ?? "" : "";
                                if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        downloadUrl = $"https://github.com/{GitHubOwner}/{GitHubRepo}/releases/download/{tagName}/RetailApp_Setup.exe";
                    }

                    return new UpdateInfo
                    {
                        Version = cleanVersion,
                        BuildNumber = cleanVersion,
                        ReleaseDate = releaseDate,
                        ReleaseNotes = releaseNotes,
                        DownloadUrl = downloadUrl,
                        IsMandatory = false
                    };
                }

                // 2. المحرك الاحتياطي: فحص Atom Feed المفتوح (يتجاوز قيود Rate Limit كلياً 100%)
                string atomUrl = $"https://github.com/{GitHubOwner}/{GitHubRepo}/releases.atom";
                var atomResponse = await _httpClient.GetAsync(atomUrl);

                if (atomResponse.IsSuccessStatusCode)
                {
                    var xmlContent = await atomResponse.Content.ReadAsStringAsync();
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlContent);

                    var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");

                    var entryNode = xmlDoc.SelectSingleNode("//atom:entry", nsmgr);
                    if (entryNode != null)
                    {
                        var idNode = entryNode.SelectSingleNode("atom:id", nsmgr);
                        var titleNode = entryNode.SelectSingleNode("atom:title", nsmgr);
                        var contentNode = entryNode.SelectSingleNode("atom:content", nsmgr);

                        string idText = idNode?.InnerText ?? "";
                        string rawTag = idText.Contains("/") ? idText.Substring(idText.LastIndexOf('/') + 1) : "";
                        string cleanVersion = rawTag.TrimStart('v', 'V');
                        string releaseNotes = contentNode?.InnerText ?? titleNode?.InnerText ?? "";
                        
                        releaseNotes = Regex.Replace(releaseNotes, "<.*?>", string.Empty).Trim();

                        string downloadUrl = $"https://github.com/{GitHubOwner}/{GitHubRepo}/releases/download/{rawTag}/RetailApp_Setup.exe";

                        return new UpdateInfo
                        {
                            Version = cleanVersion,
                            BuildNumber = cleanVersion,
                            ReleaseDate = DateTime.Now,
                            ReleaseNotes = releaseNotes,
                            DownloadUrl = downloadUrl,
                            IsMandatory = false
                        };
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DownloadUpdateAsync(string url, IProgress<double>? progress = null)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                // حماية من تحميل صفحات 404 HTML عند عدم إرفاق ملف الـ .exe
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
                if (contentType.Contains("html", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                long? totalBytes = response.Content.Headers.ContentLength;

                // إذا كان حجم الملف أقل من 50 كيلوبايت فهو ليس ملف مثبت حقيقي
                if (totalBytes.HasValue && totalBytes.Value < 50 * 1024)
                {
                    return false;
                }

                string tempPath = Path.Combine(Path.GetTempPath(), "RetailApp_Update_Installer.exe");

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                var buffer = new byte[8192];
                long totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;

                    if (totalBytes.HasValue && totalBytes.Value > 0)
                    {
                        double progressPercentage = (double)totalBytesRead / totalBytes.Value * 100;
                        progress?.Report(progressPercentage);
                    }
                }

                fileStream.Close();
                progress?.Report(100);

                // تشغيل ملف التثبيت المباشر دون فتح متصفح
                Process.Start(new ProcessStartInfo
                {
                    FileName = tempPath,
                    Arguments = "/SILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS",
                    UseShellExecute = true
                });

                Application.Current.Shutdown();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
