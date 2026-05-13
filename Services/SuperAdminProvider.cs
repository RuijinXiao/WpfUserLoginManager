using WpfUserLoginManager.Models;

namespace WpfUserLoginManager.Services;

public static class SuperAdminProvider
{
    public const string UserName = "superadmin";
    public const string Password = "SuperAdmin";

    public static AppUser GetSuperAdmin()
    {
        return new AppUser
        {
            UserId = "SUPER_ADMIN",
            UserName = UserName,
            PasswordHash = PasswordHasher.Hash(Password),
            Level = 5,
            CreatedAt = new DateTime(2026, 5, 12, 0, 0, 0, DateTimeKind.Local),
            IsSuperAdmin = true
        };
    }
}
