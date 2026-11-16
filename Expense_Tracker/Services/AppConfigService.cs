using System.Text.Json;

namespace Expense_Tracker.Services;

public static class AppConfigService
{
    private static string? _cachedBaseUrl;

    public static string GetDefaultBaseUrl()
    {
        if (!string.IsNullOrEmpty(_cachedBaseUrl))
            return _cachedBaseUrl;

        try
        {
            string json = string.Empty;

            // 1. Try reading from MAUI App Package File (Resources/Raw/appconfig.json)
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("appconfig.json").GetAwaiter().GetResult();
                using var reader = new StreamReader(stream);
                json = reader.ReadToEnd();
            }
            catch
            {
                // Fallback to direct directory file check
                var baseDir = AppContext.BaseDirectory;
                var candidatePaths = new[]
                {
                    Path.Combine(baseDir, "appconfig.json"),
                    Path.Combine(baseDir, "Resources", "Raw", "appconfig.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "appconfig.json")
                };

                foreach (var path in candidatePaths)
                {
                    if (File.Exists(path))
                    {
                        json = File.ReadAllText(path);
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

#if ANDROID
                if (root.TryGetProperty("AndroidBaseUrl", out var androidProp))
                {
                    _cachedBaseUrl = androidProp.GetString();
                }
                else if (root.TryGetProperty("ApiSettings", out var apiSettings) && apiSettings.TryGetProperty("AndroidBaseUrl", out var aProp))
                {
                    _cachedBaseUrl = aProp.GetString();
                }
#endif
                if (string.IsNullOrEmpty(_cachedBaseUrl))
                {
                    if (root.TryGetProperty("DefaultBaseUrl", out var defaultProp))
                    {
                        _cachedBaseUrl = defaultProp.GetString();
                    }
                    else if (root.TryGetProperty("ApiSettings", out var apiSettings) && apiSettings.TryGetProperty("DefaultBaseUrl", out var dProp))
                    {
                        _cachedBaseUrl = dProp.GetString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AppConfigService] Error reading appconfig.json: {ex.Message}");
        }

        // Final safety fallback if appconfig.json is absent or invalid
        if (string.IsNullOrEmpty(_cachedBaseUrl))
        {
#if ANDROID
            _cachedBaseUrl = "http://10.0.2.2:5283/";
#else
            _cachedBaseUrl = "http://localhost:5283/";
#endif
        }

        if (!_cachedBaseUrl.EndsWith('/'))
        {
            _cachedBaseUrl += "/";
        }

        return _cachedBaseUrl;
    }
}
