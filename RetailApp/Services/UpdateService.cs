using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
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
            // GitHub API يتطلب وجود Header User-Agent
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RetailApp-Updater", "1.0"));
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                string apiUrl = $"https://api.github.com/repos/{GitHubOwner}/{GitHubRepo}/releases/latest";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

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

                // البحث عن رابط التنزيل المباشر في أصول GitHub Release
                string downloadUrl = "";
                if (root.TryGetProperty("assets", out var assetsElem) && assetsElem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var asset in assetsElem.EnumerateArray())
                    {
                        string dl = asset.TryGetProperty("browser_download_url", out var dlElem) ? dlElem.GetString() ?? "" : "";
                        if (!string.IsNullOrEmpty(dl))
                        {
                            downloadUrl = dl;
                            // تفضيل الملفات التنفيذية والمضغوطة
                            string name = asset.TryGetProperty("name", out var nameElem) ? nameElem.GetString() ?? "" : "";
                            if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(downloadUrl) && root.TryGetProperty("zipball_url", out var zipElem))
                {
                    downloadUrl = zipElem.GetString() ?? "";
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
                string extension = url.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ? ".zip" : ".exe";
                string tempPath = Path.Combine(Path.GetTempPath(), $"RetailApp_Update{extension}");

                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;

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

                if (extension == ".exe")
                {
                    // تشغيل ملف التثبيت المباشر دون توجيه لأي متصفح
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = tempPath,
                        Arguments = "/SILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS",
                        UseShellExecute = true
                    });

                    Application.Current.Shutdown();
                }
                else
                {
                    // فتح مجلد الملف المحمل للمستخدم تلقائياً
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"/select,\"{tempPath}\"",
                        UseShellExecute = true
                    });
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
