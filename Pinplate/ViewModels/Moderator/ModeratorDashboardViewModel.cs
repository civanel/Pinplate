using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipesDB;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Pinplate.ViewModels;

public partial class ModeratorDashboardViewModel : ObservableObject
{
    [ObservableProperty] private int reportsCount;
    [ObservableProperty] private int reportsDelta;
    [ObservableProperty] private int spamCount;
    [ObservableProperty] private int spamDelta;
    [ObservableProperty] private int onModerationCount;
    [ObservableProperty] private int usersCount;
    [ObservableProperty] private int usersDelta;

    public ObservableCollection<ReportItem> LastReports { get; } = new();
    public ObservableCollection<LogItem> LastLogs { get; } = new();

    public ModeratorDashboardViewModel()
    {
        LoadDataFromDb();
    }

    [RelayCommand]
    private void Refresh() => LoadDataFromDb();

    [RelayCommand]
    private void OpenAllReports()
    {
        if (Application.Current.MainWindow?.DataContext is MainViewModel mainVm)
            mainVm.NavigateCommand.Execute("Reports");
    }

    [RelayCommand]
    private void OpenAllLogs()
    {
        if (Application.Current.MainWindow?.DataContext is MainViewModel mainVm)
            mainVm.NavigateCommand.Execute("Logs");
    }

    private void LoadDataFromDb()
    {
        using (var db = new DBContext())
        {
            ReportsCount = db.Complaints.Count(c => c.Status == "Открыта");
            ReportsDelta = 0;
            SpamCount = db.Complaints.Count(c => c.Reason == "Спам");
            SpamDelta = 0;
            OnModerationCount = db.Recipes.Count();
            UsersCount = db.Users.Count();
            UsersDelta = 0;

            LastReports.Clear();
            var reports = db.Complaints.OrderByDescending(c => c.CreatedAt).Take(3)
                .Select(c => new ReportItem { Id = c.ComplaintId, Title = "Жалоба #" + c.ComplaintId, Type = c.Reason }).ToList();
            foreach (var r in reports) LastReports.Add(r);

            LastLogs.Clear();
            var logs = db.Log_Entries.OrderByDescending(l => l.CreatedAt).Take(3)
                .Select(l => new LogItem { Time = l.CreatedAt.ToString("HH:mm:ss"), User = l.UserName, Action = l.Action, Status = l.Status }).ToList();
            foreach (var l in logs) LastLogs.Add(l);
        }
    }
}

public class ReportItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string DisplayText => $"{Title} — {Type}";
}

public class LogItem
{
    public string Time { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}