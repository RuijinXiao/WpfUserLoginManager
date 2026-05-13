using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfUserLoginManager;

public partial class UserEditWindow : Window
{
    private readonly bool _isEditMode;
    private bool _isPasswordVisible;
    private bool _isSyncingPassword;

    public UserEditWindow(string title, string userName = "", int level = 1, bool isEditMode = false)
    {
        InitializeComponent();
        _isEditMode = isEditMode;
        Title = string.Empty;
        UserNameInput.Text = userName;
        LevelInput.Value = Math.Clamp(level, 1, 5);
        PasswordTip.Visibility = isEditMode ? Visibility.Visible : Visibility.Collapsed;
        PasswordInput.Focus();

        if (string.IsNullOrWhiteSpace(userName))
        {
            UserNameInput.Focus();
        }
    }

    public string UserName => UserNameInput.Text.Trim();

    public string Password => _isPasswordVisible ? PasswordTextInput.Text : PasswordInput.Password;

    public int Level => (int)LevelInput.Value;

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            MessageBox.Show("请输入用户名", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
        {
            MessageBox.Show("新增用户时请输入密码", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void WindowRoot_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void TogglePasswordButton_OnClick(object sender, RoutedEventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordTextInput.Visibility = _isPasswordVisible ? Visibility.Visible : Visibility.Collapsed;
        PasswordInput.Visibility = _isPasswordVisible ? Visibility.Collapsed : Visibility.Visible;
        EyeIcon.Stroke = (Brush)FindResource(_isPasswordVisible ? "AccentBrush" : "TextBrush");

        if (_isPasswordVisible)
        {
            PasswordTextInput.Focus();
            PasswordTextInput.CaretIndex = PasswordTextInput.Text.Length;
        }
        else
        {
            PasswordInput.Focus();
        }
    }

    private void PasswordInput_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_isSyncingPassword)
        {
            return;
        }

        _isSyncingPassword = true;
        PasswordTextInput.Text = PasswordInput.Password;
        _isSyncingPassword = false;
    }

    private void PasswordTextInput_OnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (_isSyncingPassword)
        {
            return;
        }

        _isSyncingPassword = true;
        PasswordInput.Password = PasswordTextInput.Text;
        _isSyncingPassword = false;
    }

    private void LevelInput_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (LevelText is not null)
        {
            LevelText.Text = $"当前等级：{(int)e.NewValue}";
        }
    }
}
