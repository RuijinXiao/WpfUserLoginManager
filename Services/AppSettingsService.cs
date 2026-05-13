using System.IO;
using System.Text.Json;
using WpfUserLoginManager.Models;

namespace WpfUserLoginManager.Services;

public sealed class AppSettingsService
{
    public string FilePath { get; } = Path.Combine(AppContext.BaseDirectory, "Config", "appsettings.json");

    public AppSettings Load()
    {
        if (!File.Exists(FilePath))
        {
            var defaults = new AppSettings();
            Save(defaults);
            return defaults;
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, options));
    }
}
