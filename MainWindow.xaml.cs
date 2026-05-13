using System.Windows;
using System.Windows.Input;
using WpfUserLoginManager.ViewModels;

namespace WpfUserLoginManager;

public partial class MainWindow : Window
{
    private readonly LoginViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.LoginSucceeded += OnLoginSucceeded;
    }

    private void PasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = PasswordInput.Password;
    }

    private void ExitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void WindowRoot_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void OnLoginSucceeded(Models.AppUser user)
    {
        bool isOPeningUserManagement = false;
        if (user.Level == 5 && !user.IsSuperAdmin)
        {
            var result = MessageBox.Show("登录成功！您是5级管理员，可以进入用户管理界面。选择是，进入用户管理界面。选择否，将关闭主界面，登录软件", "登录成功", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                isOPeningUserManagement = true;
            }

        }
        else 
        {
            MessageBox.Show("登录成功！", "登录成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        if (user.IsSuperAdmin || isOPeningUserManagement)
        {
            var userManagementWindow = new UserManagementWindow(user);
            userManagementWindow.Closed += (_, _) =>
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                Close();
            };
            Hide();
            userManagementWindow.Show();
            return;
        }


        Close();
    }
}
