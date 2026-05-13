using System.Windows;
using System.Windows.Input;
using WpfUserLoginManager.Commands;
using WpfUserLoginManager.Models;
using WpfUserLoginManager.Services;

namespace WpfUserLoginManager.ViewModels;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly AuthenticationService _authenticationService;
    private readonly LoginRecordService _loginRecordService;
    private string _userName = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _requirePassword = true;

    public LoginViewModel()
    {
        var userCsvService = new UserCsvService();
        var appSettingsService = new AppSettingsService();
        _authenticationService = new AuthenticationService(userCsvService, appSettingsService);
        _loginRecordService = new LoginRecordService();
        RequirePassword = appSettingsService.Load().RequirePassword;
        LoginCommand = new RelayCommand(_ => Login());
    }

    public event Action<AppUser>? LoginSucceeded;

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool RequirePassword
    {
        get => _requirePassword;
        set => SetProperty(ref _requirePassword, value);
    }

    public ICommand LoginCommand { get; }

    private void Login()
    {
        ErrorMessage = string.Empty;
        var user = _authenticationService.Login(UserName.Trim(), Password);
        if (user is null)
        {
            ErrorMessage = RequirePassword ? "用户名或密码错误" : "用户名不存在";
            return;
        }

        if (!user.IsSuperAdmin)
        {
            _loginRecordService.Save(new LoginRecord
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Level = user.Level,
                IsSuperAdmin = user.IsSuperAdmin,
                LoginAt = DateTime.Now
            });
        }

       
        LoginSucceeded?.Invoke(user);
    }
}
