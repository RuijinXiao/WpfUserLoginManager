namespace WpfUserLoginManager.Models;

public sealed class LoginRecord
{
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public int Level { get; set; }

    public bool IsSuperAdmin { get; set; }

    public DateTime LoginAt { get; set; } = DateTime.Now;
}
