using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using RecipesDB;

namespace Pinplate.ViewModels.Expert;

public partial class ExpertRejectedViewModel : ObservableObject
{
    public ObservableCollection<RejectedItem> Recipes { get; } = new();

    public ExpertRejectedViewModel() => LoadDataFromDb();

    [RelayCommand]
    private void Return(RejectedItem? item)
    {
        if (item is null || item.Id == 0) return;

        using (var db = new DBContext())
        {
            var recipe = db.Recipes.FirstOrDefault(r => r.RecipeId == item.Id);
            if (recipe != null)
            {
                // Возвращаем на модерацию
                recipe.Status = "Pending";
                recipe.RejectReason = null;
                recipe.RejectedAt = null;
                db.SaveChanges();
            }
        }
        LoadDataFromDb();
    }

    private void LoadDataFromDb()
    {
        Recipes.Clear();

        using (var db = new DBContext())
        {
            var rejectedRecipes = db.Recipes
                .Where(r => r.Status == "Rejected")
                .Select(r => new RejectedItem
                {
                    Id = r.RecipeId,
                    Title = r.Name,
                    Author = r.Author != null ? r.Author.Name : "Неизвестен",
                    Reason = r.RejectReason ?? "Причина не указана",
                    Date = r.RejectedAt.HasValue
                        ? r.RejectedAt.Value.ToString("dd.MM.yyyy")
                        : "—"
                })
                .ToList();

            foreach (var item in rejectedRecipes)
            {
                Recipes.Add(item);
            }
        }
    }
}

public class RejectedItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Reason { get; set; } = "";
    public string Date { get; set; } = "";
}