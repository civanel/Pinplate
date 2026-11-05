using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pinplate.Views.Moderator;
using Pinplate.Views.Settings;
using Pinplate.Views.Users;
using System.Collections.ObjectModel; // ВАЖНО

namespace Pinplate.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? currentView;

    [ObservableProperty]
    private string currentSectionTitle = "Панель модератора";

    [ObservableProperty]
    private string activeMenuItem = "Dashboard";

    // КОЛЛЕКЦИЯ ДЛЯ ИСТОРИИ ПОИСКА
    public ObservableCollection<string> SearchHistory { get; } = new();

    public MainViewModel()
    {
        Navigate("Dashboard");
    }

    [RelayCommand]
    private void Navigate(string destination)
    {
        ActiveMenuItem = destination;

        (CurrentView, CurrentSectionTitle) = destination switch
        {
            "Dashboard" => (new ModeratorDashboard(), "Панель модератора"),
            "Reports" => (new ReportsView(), "Жалобы"),
            "Statistics" => (new StatisticsView(), "Статистика"),
            "Users" => (new UsersListView(), "Юзеры"),
            "Logs" => (new LogsView(), "Логи"),
            "Settings" => (new SettingsView(), "Настройки"),
            _ => (CurrentView, CurrentSectionTitle)
        };
    }

    // ОБНОВЛЕННЫЙ МЕТОД ПОИСКА
    public void GlobalSearch(string query)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            if (SearchHistory.Contains(query)) SearchHistory.Remove(query);
            SearchHistory.Insert(0, query);
            if (SearchHistory.Count > 5) SearchHistory.RemoveAt(5);
        }

        if (CurrentView is Pinplate.Views.Users.UsersListView usersView && usersView.DataContext is UsersListViewModel usersVm)
            usersVm.SearchQuery = query;
        else if (CurrentView is Pinplate.Views.Moderator.LogsView logsView && logsView.DataContext is LogsViewModel logsVm)
            logsVm.SearchQuery = query;
        else if (CurrentView is Pinplate.Views.Moderator.ReportsView reportsView && reportsView.DataContext is ReportsViewModel reportsVm)
            reportsVm.SearchQuery = query;
    }
}