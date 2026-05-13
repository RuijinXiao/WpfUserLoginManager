using System.Windows;
using System.Windows.Input;
using WpfUserLoginManager.Models;
using WpfUserLoginManager.ViewModels;

namespace WpfUserLoginManager;

public partial class UserManagementWindow : Window
{
    public UserManagementWindow(AppUser currentUser)
    {
        InitializeComponent();
        DataContext = new UserManagementViewModel(currentUser);
    }

    private void WindowRoot_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void ExitButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
