using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pinplate.Views.Moderator.Dialogs;
using RecipesDB;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pinplate.ViewModels;

public partial class UsersListViewModel : ObservableObject
{
    public ObservableCollection<UserCard> Users { get; } = new();

    [ObservableProperty] private string selectedSort = "По дате";
    [ObservableProperty] private string selectedRole = "Все";

    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            LoadDataFromDb();
        }
    }

    public UsersListViewModel() => LoadDataFromDb();

    partial void OnSelectedSortChanged(string value) => LoadDataFromDb();
    partial void OnSelectedRoleChanged(string value) => LoadDataFromDb();

    [RelayCommand]
    private void Reset()
    {
        SearchQuery = "";
        SelectedRole = "Все";
        SelectedSort = "По дате";
    }

    [RelayCommand]
    private void Profile(UserCard? item)
    {
        if (item is null) return;

        var profileWindow = new Window
        {
            Title = "Профиль пользователя",
            Width = 300,
            Height = 350,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            Background = System.Windows.Media.Brushes.White
        };

        var stackPanel = new StackPanel { Margin = new Thickness(20) };
        var avatarBorder = new Border
        {
            Width = 150,
            Height = 150,
            CornerRadius = new CornerRadius(75),
            Background = System.Windows.Media.Brushes.LightCoral,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 15)
        };
        avatarBorder.Child = new TextBlock
        {
            Text = item.Initial,
            FontSize = 64,
            FontWeight = FontWeights.Bold,
            Foreground = System.Windows.Media.Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        stackPanel.Children.Add(avatarBorder);
        stackPanel.Children.Add(new TextBlock { Text = item.FullName, FontSize = 18, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center });
        stackPanel.Children.Add(new TextBlock { Text = item.Username, FontSize = 14, Foreground = System.Windows.Media.Brushes.Gray, Margin = new Thickness(0, 5, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });

        profileWindow.Content = stackPanel;
        profileWindow.ShowDialog();
    }

    [RelayCommand]
    private void Ban(UserCard? item)
    {
        if (item is null || item.Id == 0) return;

        var dialog = new BanUserDialog(item.Id, item.FullName);
        dialog.Owner = Application.Current.MainWindow;

        if (dialog.ShowDialog() == true)
        {
            LoadDataFromDb();
        }
    }

    private void LoadDataFromDb()
    {
        Users.Clear();
        using (var db = new DBContext())
        {
            var query = db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(q) || u.Email.ToLower().Contains(q));
            }

            if (SelectedRole != "Все")
            {
                query = query.Where(u => u.Role == SelectedRole);
            }

            if (SelectedSort == "По дате")
                query = query.OrderByDescending(u => u.CreatedAt);
            else if (SelectedSort == "По имени")
                query = query.OrderBy(u => u.Name);

            var usersFromDb = query.ToList();

            foreach (var user in usersFromDb)
            {
                Users.Add(new UserCard
                {
                    Id = user.UserId,
                    Initial = string.IsNullOrWhiteSpace(user.Name) ? "?" : user.Name.Substring(0, 1).ToUpper(),
                    Username = "@" + user.Name.ToLower(),
                    FullName = user.Name,
                    Info = $"зарегистрирован {user.CreatedAt:dd.MM.yyyy} • Роль: {user.Role}"
                });
            }
        }
    }
}

public class UserCard
{
    public int Id { get; set; }
    public string Initial { get; set; } = "";
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Info { get; set; } = "";
}