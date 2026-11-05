using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using RecipesDB;
using System.Windows;

namespace Pinplate.ViewModels.Expert;

public partial class ExpertReviewViewModel : ObservableObject
{
    public ObservableCollection<RecipeItem> Recipes { get; } = new();

    [ObservableProperty] private string selectedSort = "По дате";

    // ДОБАВЛЯЕМ ПОИСК
    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set { _searchQuery = value; LoadDataFromDb(); }
    }

    public ExpertReviewViewModel() => LoadDataFromDb();

    partial void OnSelectedSortChanged(string value) => LoadDataFromDb();

    [RelayCommand]
    private void Approve(RecipeItem? item)
    {
        if (item is null || item.Id == 0) return;

        using (var db = new DBContext())
        {
            var recipe = db.Recipes.FirstOrDefault(r => r.RecipeId == item.Id);
            if (recipe != null)
            {
                recipe.Status = "Approved";
                recipe.ApprovedAt = System.DateTime.UtcNow;
                db.SaveChanges();
            }
        }
        LoadDataFromDb();
    }

    [RelayCommand]
    private void Reject(RecipeItem? item)
    {
        if (item is null) return;

        var dialog = new Pinplate.Views.Expert.Dialogs.RejectRecipeDialog();
        dialog.Owner = Application.Current.MainWindow;

        if (dialog.ShowDialog() == true)
        {
            using (var db = new DBContext())
            {
                var recipe = db.Recipes.FirstOrDefault(r => r.RecipeId == item.Id);
                if (recipe != null)
                {
                    recipe.Status = "Rejected";
                    recipe.RejectReason = dialog.RejectReason;
                    recipe.RejectedAt = System.DateTime.UtcNow;
                    db.SaveChanges();
                }
            }
            LoadDataFromDb();
        }
    }

    [RelayCommand]
    private void Reset()
    {
        SelectedSort = "По дате";
        SearchQuery = "";
    }

    private void LoadDataFromDb()
    {
        Recipes.Clear();

        using (var db = new DBContext())
        {
            var query = db.Recipes.Where(r => r.Status == "Pending");

            // ФИЛЬТРАЦИЯ
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(q));
            }

            if (SelectedSort == "По дате")
                query = query.OrderByDescending(r => r.CreatedAt);
            else if (SelectedSort == "По времени готовки")
                query = query.OrderBy(r => r.CookTimeMinutes);

            var reviewRecipes = query
                .Select(r => new RecipeItem
                {
                    Id = r.RecipeId,
                    Title = r.Name,
                    Description = r.Description,
                    Author = r.Author != null ? r.Author.Name : "Неизвестен",
                    Category = r.RecipeCategories.FirstOrDefault() != null
                        ? r.RecipeCategories.FirstOrDefault()!.Category.Name
                        : "Без категории",
                    Time = r.CookTimeMinutes + " мин"
                })
                .ToList();

            foreach (var item in reviewRecipes) Recipes.Add(item);
        }
    }
}

public class RecipeItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Author { get; set; } = "";
    public string Category { get; set; } = "";
    public string Time { get; set; } = "";
}