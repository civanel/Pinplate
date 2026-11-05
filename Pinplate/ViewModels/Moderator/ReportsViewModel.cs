using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using RecipesDB;
using System.Windows;
using Pinplate.Views.Moderator.Dialogs;

namespace Pinplate.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    [ObservableProperty] private string selectedFilter = "Открыта";
    [ObservableProperty] private string selectedType = "Все";
    [ObservableProperty] private string selectedPeriod = "По дате";

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

    public ObservableCollection<ComplaintItem> Complaints { get; } = new();

    public ReportsViewModel() => LoadDataFromDb();

    partial void OnSelectedFilterChanged(string value) => LoadDataFromDb();
    partial void OnSelectedTypeChanged(string value) => LoadDataFromDb();

    [RelayCommand]
    private void Reset()
    {
        SelectedFilter = "Открыта";
        SelectedType = "Все";
        SelectedPeriod = "По дате";
        SearchQuery = "";
        LoadDataFromDb();
    }

    [RelayCommand]
    private void Hide(ComplaintItem? item)
    {
        if (item is null || item.Id == 0) return;

        using (var db = new DBContext())
        {
            var complaint = db.Complaints.FirstOrDefault(c => c.ComplaintId == item.Id);
            if (complaint != null)
            {
                complaint.Status = "Решено";
                db.SaveChanges();
            }
        }
        LoadDataFromDb();
    }

    [RelayCommand]
    private void Reply(ComplaintItem? item)
    {
        if (item is null || item.Id == 0) return;

        // ПЕРЕДАЕМ ПАРАМЕТРЫ ПРАВИЛЬНО
        var dialog = new ReplyDialog(item.Id, item.User, item.Description);
        dialog.Owner = Application.Current.MainWindow;

        if (dialog.ShowDialog() == true)
        {
            LoadDataFromDb();
        }
    }

    private void LoadDataFromDb()
    {
        Complaints.Clear();

        using (var db = new DBContext())
        {
            var query = db.Complaints.AsQueryable();

            if (SelectedFilter != "Все") query = query.Where(c => c.Status == SelectedFilter);
            if (SelectedType != "Все") query = query.Where(c => c.Reason == SelectedType);

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLower();
                query = query.Where(c => c.Reporter.Name.ToLower().Contains(q) || c.Reason.ToLower().Contains(q));
            }

            var complaintsFromDb = query
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ComplaintItem
                {
                    Id = c.ComplaintId,
                    Time = c.CreatedAt.ToString("HH:mm:ss"),
                    User = c.Reporter.Name,
                    Description = "Жалоба #" + c.ComplaintId + " — " + c.Reason,
                    Status = c.Status
                })
                .ToList();

            foreach (var item in complaintsFromDb) Complaints.Add(item);
        }
    }
}

public class ComplaintItem
{
    public int Id { get; set; }
    public string Time { get; set; } = "";
    public string User { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
}