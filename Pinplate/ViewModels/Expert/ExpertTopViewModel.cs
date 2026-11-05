using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using RecipesDB;
using System.Windows;

namespace Pinplate.ViewModels.Expert;

public partial class ExpertTopViewModel : ObservableObject
{
    public ObservableCollection<TopRecipeItem> Recipes { get; } = new();

    [ObservableProperty] private string selectedSort = "По дате";
    [ObservableProperty] private string selectedFilter = "Все";

    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set { _searchQuery = value; LoadDataFromDb(); }
    }

    public ExpertTopViewModel() => LoadDataFromDb();

    partial void OnSelectedSortChanged(string value) => LoadDataFromDb();
    partial void OnSelectedFilterChanged(string value) => LoadDataFromDb();

    [RelayCommand]
    private void ToggleTop(TopRecipeItem? item)
    {
        if (item is null) return;

        using (var db = new DBContext())
        {
            var recipe = db.Recipes.FirstOrDefault(r => r.RecipeId == item.Id);
            if (recipe != null)
            {
                // Если в топе - убираем, если нет - добавляем
                recipe.IsTop = !recipe.IsTop;
                db.SaveChanges();
            }
        }
        LoadDataFromDb();
    }

    [RelayCommand]
    private void Reset()
    {
        SelectedSort = "По дате";
        SelectedFilter = "Все";
        SearchQuery = "";
    }

    private void LoadDataFromDb()
    {
        Recipes.Clear();
        using (var db = new DBContext())
        {
            var query = db.Recipes.Where(r => r.Status == "Approved");

            if (SelectedFilter == "В топе") query = query.Where(r => r.IsTop == true);

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(q));
            }

            if (SelectedSort == "По дате") query = query.OrderByDescending(r => r.ApprovedAt);
            else if (SelectedSort == "По рейтингу") query = query.OrderByDescending(r => r.RecipeReviews.Any() ? r.RecipeReviews.Average(rv => rv.Rating) : 0);

            var list = query.Select(r => new TopRecipeItem
            {
                Id = r.RecipeId,
                Title = r.Name,
                Description = r.Description,
                Author = r.Author != null ? r.Author.Name : "Неизвестен",
                Rating = r.RecipeReviews.Any() ? r.RecipeReviews.Average(rv => rv.Rating) : 0,
                ApprovedDate = r.ApprovedAt.HasValue ? r.ApprovedAt.Value.ToString("dd.MM.yyyy") : "—",
                IsTop = r.IsTop
            }).ToList();

            foreach (var item in list) Recipes.Add(item);
        }
    }
}

public class TopRecipeItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Author { get; set; } = "";
    public double Rating { get; set; }
    public string ApprovedDate { get; set; } = "";
    public bool IsTop { get; set; }
}