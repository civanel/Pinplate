using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using RecipesDB;
using System;
using System.Windows;
using Pinplate.Views.Moderator.Dialogs;

namespace Pinplate.ViewModels;

public partial class LogsViewModel : ObservableObject
{
    public ObservableCollection<LogRow> Logs { get; } = new();

    [ObservableProperty] private string selectedStatus = "Все";
    [ObservableProperty] private string selectedSort = "По дате";

    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set { _searchQuery = value; LoadDataFromDb(); }
    }

    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private int pageSize = 10;
    [ObservableProperty] private int totalPages = 1;
    [ObservableProperty] private string pageInfo = "";

    public LogsViewModel() => LoadDataFromDb();

    partial void OnSelectedStatusChanged(string value) { CurrentPage = 1; LoadDataFromDb(); }
    partial void OnSelectedSortChanged(string value) { CurrentPage = 1; LoadDataFromDb(); }

    [RelayCommand]
    private void Reset()
    {
        SelectedStatus = "Все";
        SelectedSort = "По дате";
        SearchQuery = "";
        CurrentPage = 1;
        LoadDataFromDb();
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            LoadDataFromDb();
        }
    }

    [RelayCommand]
    private void PrevPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            LoadDataFromDb();
        }
    }

    [RelayCommand]
    private void GoToPage(string pageStr)
    {
        if (int.TryParse(pageStr, out int page) && page >= 1 && page <= TotalPages)
        {
            CurrentPage = page;
            LoadDataFromDb();
        }
    }

    [RelayCommand]
    private void Reply(LogRow? item)
    {
        if (item is null || item.Id == 0) return;

        var dialog = new ReplyDialog(item.Id, item.User, $"Ответ на лог: {item.Action}");
        dialog.Owner = Application.Current.MainWindow;

        if (dialog.ShowDialog() == true)
        {
            using (var db = new DBContext())
            {
                var logDb = db.Log_Entries.FirstOrDefault(l => l.LogId == item.Id);
                if (logDb != null)
                {
                    logDb.Status = "Решено";
                    db.SaveChanges();
                }
            }
            LoadDataFromDb();
        }
    }

    private void LoadDataFromDb()
    {
        Logs.Clear();

        using (var db = new DBContext())
        {
            var query = db.Log_Entries.AsQueryable();

            if (SelectedStatus != "Все")
            {
                query = query.Where(l => l.Status == SelectedStatus);
            }

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLower();
                query = query.Where(l => l.UserName.ToLower().Contains(q) || l.Action.ToLower().Contains(q));
            }

            // ИСПРАВЛЕННАЯ СОРТИРОВКА
            if (SelectedSort == "По дате")
                query = query.OrderByDescending(l => l.CreatedAt);
            else if (SelectedSort == "По юзеру")
                query = query.OrderBy(l => l.UserName);

            int totalCount = query.Count();
            TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;

            var logsFromDb = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(l => new LogRow
                {
                    Id = l.LogId,
                    Time = l.CreatedAt.ToString("HH:mm:ss"),
                    User = l.UserName,
                    Action = l.Action,
                    Status = l.Status
                })
                .ToList();

            foreach (var log in logsFromDb)
            {
                Logs.Add(log);
            }

            int start = totalCount > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
            int end = Math.Min(CurrentPage * PageSize, totalCount);
            PageInfo = totalCount > 0 ? $"Показано {start}-{end} из {totalCount}" : "Нет записей";
        }
    }
}

public class LogRow
{
    public int Id { get; set; }
    public string Time { get; set; } = "";
    public string User { get; set; } = "";
    public string Action { get; set; } = "";
    public string Status { get; set; } = "";
}