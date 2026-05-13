using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using WpfUserLoginManager.Models;

namespace WpfUserLoginManager.Services;

public sealed class LoginRecordService
{
    public string FilePath { get; } = Path.Combine(AppContext.BaseDirectory, "Result", "current_login.json");

    public void Save(LoginRecord record)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        File.WriteAllText(FilePath, JsonSerializer.Serialize(record, options));
    }
}
