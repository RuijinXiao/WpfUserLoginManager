using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WpfUserLoginManager.Commands;
using WpfUserLoginManager.Models;
using WpfUserLoginManager.Services;

namespace WpfUserLoginManager.ViewModels;

public sealed class UserManagementViewModel : ViewModelBase
{
    private readonly UserCsvService _userCsvService = new();
    private readonly AppSettingsService _appSettingsService = new();
    private AppUser? _selectedUser;
    private string _searchKeyword = string.Empty;
    private bool _requirePassword;

    public UserManagementViewModel(AppUser currentUser)
    {
        CurrentUser = currentUser;
        RequirePassword = _appSettingsService.Load().RequirePassword;
        Users = new ObservableCollection<AppUser>(_userCsvService.LoadUsers());
        AddCommand = new RelayCommand(parameter => AddUser(parameter as Window));
        UpdateCommand = new RelayCommand(parameter => UpdateUser(parameter as Window), _ => SelectedUser is not null);
        DeleteCommand = new RelayCommand(_ => DeleteUser(), _ => SelectedUser is not null);
        SearchCommand = new RelayCommand(_ => SearchUsers());
        RefreshCommand = new RelayCommand(_ => RefreshUsers());
        ClearCommand = new RelayCommand(_ => ClearInputs());
    }

    public AppUser CurrentUser { get; }

    public ObservableCollection<AppUser> Users { get; }

    public AppUser? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (SetProperty(ref _selectedUser, value))
            {
                RaiseCommandStates();
            }
        }
    }

    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword, value);
    }

    public bool RequirePassword
    {
        get => _requirePassword;
        set
        {
            if (!SetProperty(ref _requirePassword, value))
            {
                return;
            }

            _appSettingsService.Save(new AppSettings { RequirePassword = value });
        }
    }

    public ICommand AddCommand { get; }

    public ICommand UpdateCommand { get; }

    public ICommand DeleteCommand { get; }

    public ICommand SearchCommand { get; }

    public ICommand RefreshCommand { get; }

    public ICommand ClearCommand { get; }

    private void AddUser(Window? owner)
    {
        var dialog = new UserEditWindow("新增用户") { Owner = owner };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        if (Users.Any(user => string.Equals(user.UserName, dialog.UserName, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("用户名已存在", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!Confirm("确认新增这个用户？"))
        {
            return;
        }

        Users.Add(new AppUser
        {
            UserName = dialog.UserName,
            PasswordHash = PasswordHasher.Hash(dialog.Password),
            Level = dialog.Level,
            CreatedAt = DateTime.Now
        });

        SaveAndRefresh();
        ClearInputs();
    }

    private void UpdateUser(Window? owner)
    {
        if (SelectedUser is null)
        {
            return;
        }

        var dialog = new UserEditWindow("修改用户", SelectedUser.UserName, SelectedUser.Level, isEditMode: true) { Owner = owner };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        if (Users.Any(user => user.UserId != SelectedUser.UserId
                              && string.Equals(user.UserName, dialog.UserName, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("用户名已存在", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!Confirm("确认修改选中的用户？"))
        {
            return;
        }

        SelectedUser.UserName = dialog.UserName;
        SelectedUser.Level = dialog.Level;
        if (!string.IsNullOrWhiteSpace(dialog.Password))
        {
            SelectedUser.PasswordHash = PasswordHasher.Hash(dialog.Password);
        }

        SaveAndRefresh();
        ClearInputs();
    }

    private void DeleteUser()
    {
        if (SelectedUser is null || !Confirm("确认删除选中的用户？"))
        {
            return;
        }

        Users.Remove(SelectedUser);
        SaveAndRefresh();
        ClearInputs();
    }

    private void SearchUsers()
    {
        if (string.IsNullOrWhiteSpace(SearchKeyword))
        {
            RefreshUsers();
            return;
        }

        if (!Confirm("确认按当前关键词查询？"))
        {
            return;
        }

        var keyword = SearchKeyword.Trim();
        var matched = _userCsvService.LoadUsers()
            .Where(user => user.UserName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                           || user.UserId.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
        ReplaceUsers(matched);
    }

    private void RefreshUsers()
    {
        ReplaceUsers(_userCsvService.LoadUsers());
    }

    private void ClearInputs()
    {
        SelectedUser = null;
        SearchKeyword = string.Empty;
    }

    private void SaveAndRefresh()
    {
        _userCsvService.SaveUsers(Users);
        RefreshUsers();
    }

    private void ReplaceUsers(IEnumerable<AppUser> users)
    {
        Users.Clear();
        foreach (var user in users)
        {
            Users.Add(user);
        }

        RaiseCommandStates();
    }

    private static bool Confirm(string message)
    {
        return MessageBox.Show(message, "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    private void RaiseCommandStates()
    {
        (UpdateCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
