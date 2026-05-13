namespace WpfUserLoginManager.Models;

public sealed class AppUser
{
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int Level { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? LastLoginAt { get; set; }

    public bool IsSuperAdmin { get; set; }
}
