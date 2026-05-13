using WpfUserLoginManager.Models;

namespace WpfUserLoginManager.Services;

public sealed class AuthenticationService
{
    private readonly UserCsvService _userCsvService;
    private readonly AppSettingsService _appSettingsService;

    public AuthenticationService(UserCsvService userCsvService, AppSettingsService appSettingsService)
    {
        _userCsvService = userCsvService;
        _appSettingsService = appSettingsService;
    }

    public AppUser? Login(string userName, string password)
    {
        var settings = _appSettingsService.Load();
        var superAdmin = SuperAdminProvider.GetSuperAdmin();
        if (string.Equals(userName, superAdmin.UserName, StringComparison.OrdinalIgnoreCase)
            && (!settings.RequirePassword || PasswordHasher.Verify(password, superAdmin.PasswordHash)))
        {
            superAdmin.LastLoginAt = DateTime.Now;
            return superAdmin;
        }

        var users = _userCsvService.LoadUsers();
        var user = users.FirstOrDefault(item => string.Equals(item.UserName, userName, StringComparison.OrdinalIgnoreCase));
        if (user is null || (settings.RequirePassword && !PasswordHasher.Verify(password, user.PasswordHash)))
        {
            return null;
        }

        user.LastLoginAt = DateTime.Now;
        _userCsvService.SaveUsers(users);
        return user;
    }
}
