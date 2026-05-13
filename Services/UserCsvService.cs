using System.Globalization;
using System.IO;
using System.Text;
using WpfUserLoginManager.Models;

namespace WpfUserLoginManager.Services;

public sealed class UserCsvService
{
    private static readonly string[] Headers =
    [
        "UserId",
        "UserName",
        "PasswordHash",
        "Level",
        "CreatedAt",
        "LastLoginAt"
    ];

    public string FilePath { get; }

    public UserCsvService()
    {
        FilePath = Path.Combine(AppContext.BaseDirectory, "Data", "users.csv");
    }

    public List<AppUser> LoadUsers()
    {
        EnsureFile();
        var users = new List<AppUser>();
        var lines = File.ReadAllLines(FilePath, Encoding.UTF8).Skip(1);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = ParseCsvLine(line);
            if (columns.Count < Headers.Length)
            {
                continue;
            }

            users.Add(new AppUser
            {
                UserId = (users.Count + 1).ToString(CultureInfo.InvariantCulture),
                UserName = columns[1],
                PasswordHash = columns[2],
                Level = int.TryParse(columns[3], out var level) ? Math.Clamp(level, 1, 5) : 1,
                CreatedAt = ParseDate(columns[4]) ?? DateTime.Now,
                LastLoginAt = ParseDate(columns[5])
            });
        }

        return users;
    }

    public void SaveUsers(IEnumerable<AppUser> users)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        var lines = new List<string> { string.Join(",", Headers) };
        var index = 1;
        var normalizedUsers = users
            .Where(user => !user.IsSuperAdmin)
            .Select(user =>
            {
                user.UserId = index.ToString(CultureInfo.InvariantCulture);
                index++;
                return user;
            });
        lines.AddRange(normalizedUsers.Select(ToCsvLine));
        File.WriteAllLines(FilePath, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    private void EnsureFile()
    {
        if (File.Exists(FilePath))
        {
            return;
        }

        SaveUsers([]);
    }

    private static string ToCsvLine(AppUser user)
    {
        return string.Join(",",
            Escape(user.UserId),
            Escape(user.UserName),
            Escape(user.PasswordHash),
            user.Level.ToString(CultureInfo.InvariantCulture),
            Escape(FormatDate(user.CreatedAt)),
            Escape(user.LastLoginAt.HasValue ? FormatDate(user.LastLoginAt.Value) : string.Empty));
    }

    private static string FormatDate(DateTime value)
    {
        return value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    private static DateTime? ParseDate(string value)
    {
        if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        return null;
    }

    private static string Escape(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var builder = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var current = line[i];
            if (current == '"' && inQuotes && i + 1 < line.Length && line[i + 1] == '"')
            {
                builder.Append('"');
                i++;
            }
            else if (current == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (current == ',' && !inQuotes)
            {
                result.Add(builder.ToString());
                builder.Clear();
            }
            else
            {
                builder.Append(current);
            }
        }

        result.Add(builder.ToString());
        return result;
    }
}
