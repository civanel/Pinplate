using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using RecipesDB;

namespace Pinplate.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    [ObservableProperty] private string selectedPeriod = "Месяц";

    public ObservableCollection<System.Windows.Point> ChartPoints { get; } = new();
    public ObservableCollection<TopItem> TopRecipes { get; } = new();
    public ObservableCollection<TopItem> TopUsers { get; } = new();
    public ObservableCollection<DayActivity> ActivityByDay { get; } = new();

    public StatisticsViewModel() => LoadDataFromDb();

    partial void OnSelectedPeriodChanged(string value) => LoadDataFromDb();

    [RelayCommand]
    private void Export()
    {
        try
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV файл (*.csv)|*.csv",
                FileName = $"Статистика_{SelectedPeriod}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                using (var writer = new StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("=== ТОП РЕЦЕПТОВ ===");
                    writer.WriteLine("Место;Название;В избранном");
                    foreach (var r in TopRecipes)
                        writer.WriteLine($"{r.Rank};{r.Name};{r.Value}");

                    writer.WriteLine();
                    writer.WriteLine("=== ТОП ЮЗЕРОВ ===");
                    writer.WriteLine("Место;Имя;Отзывов");
                    foreach (var u in TopUsers)
                        writer.WriteLine($"{u.Rank};{u.Name};{u.Value}");

                    writer.WriteLine();
                    writer.WriteLine("=== АКТИВНОСТЬ ПО ДНЯМ ===");
                    writer.WriteLine("День;Количество");
                    foreach (var d in ActivityByDay)
                        writer.WriteLine($"{d.Day};{d.Value}");
                }

                MessageBox.Show("Статистика успешно экспортирована!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadDataFromDb()
    {
        using (var db = new DBContext())
        {
            // 1. ГРАФИК (зависит от выбранного периода)
            ChartPoints.Clear();
            int days = SelectedPeriod switch
            {
                "Неделя" => 7,
                "Месяц" => 30,
                "Год" => 365,
                _ => 30
            };
            var startDate = System.DateTime.UtcNow.Date.AddDays(-days);

            var recipesPerDay = db.Recipes
                .Where(r => r.CreatedAt >= startDate)
                .GroupBy(r => r.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            double maxCount = recipesPerDay.Any() ? recipesPerDay.Max(x => x.Count) : 1;
            double w = 600, h = 100;
            int pointCount = recipesPerDay.Count > 0 ? recipesPerDay.Count : 1;

            for (int i = 0; i < recipesPerDay.Count; i++)
            {
                var dayData = recipesPerDay[i];
                double x = i * (w / (pointCount > 1 ? pointCount - 1 : 1));
                double y = h - (dayData.Count / maxCount * h);
                ChartPoints.Add(new System.Windows.Point(x, y));
            }

            // 2. ТОП РЕЦЕПТОВ (по количеству добавлений в избранное)
            TopRecipes.Clear();
            var topRecipesFromDb = db.Recipes
                .OrderByDescending(r => r.UserFavorites.Count)
                .Take(4)
                .Select(r => new { r.Name, Count = r.UserFavorites.Count })
                .ToList();

            int rank = 1;
            foreach (var item in topRecipesFromDb)
            {
                TopRecipes.Add(new TopItem { Rank = rank++, Name = item.Name, Value = item.Count });
            }

            // 3. ТОП ПОЛЬЗОВАТЕЛЕЙ (по количеству отзывов)
            TopUsers.Clear();
            var topUsersFromDb = db.Users
                .OrderByDescending(u => u.RecipeReviews.Count)
                .Take(3)
                .Select(u => new { u.Name, Count = u.RecipeReviews.Count })
                .ToList();

            rank = 1;
            foreach (var item in topUsersFromDb)
            {
                TopUsers.Add(new TopItem { Rank = rank++, Name = "@" + item.Name.ToLower(), Value = item.Count });
            }

            // 4. АКТИВНОСТЬ ПО ДНЯМ (по логам за период)
            ActivityByDay.Clear();
            var activity = db.Log_Entries
                .Where(l => l.CreatedAt >= startDate)
                .GroupBy(l => l.CreatedAt.DayOfWeek)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToList();

            int maxActivity = activity.Any() ? activity.Max(a => a.Count) : 1;

            var orderedDays = new[]
            {
                System.DayOfWeek.Monday, System.DayOfWeek.Tuesday, System.DayOfWeek.Wednesday,
                System.DayOfWeek.Thursday, System.DayOfWeek.Friday, System.DayOfWeek.Saturday,
                System.DayOfWeek.Sunday
            };

            foreach (var day in orderedDays)
            {
                var dayData = activity.FirstOrDefault(a => a.Day == day);
                int count = dayData?.Count ?? 0;

                string dayName = day switch
                {
                    System.DayOfWeek.Monday => "Пн",
                    System.DayOfWeek.Tuesday => "Вт",
                    System.DayOfWeek.Wednesday => "Ср",
                    System.DayOfWeek.Thursday => "Чт",
                    System.DayOfWeek.Friday => "Пт",
                    System.DayOfWeek.Saturday => "Сб",
                    System.DayOfWeek.Sunday => "Вс",
                    _ => "??"
                };

                ActivityByDay.Add(new DayActivity { Day = dayName, Value = count, Max = maxActivity });
            }
        }
    }
}

public class TopItem
{
    public int Rank { get; set; }
    public string Name { get; set; } = "";
    public int Value { get; set; }
    public string DisplayText => $"{Rank}. {Name} — {Value}";
}

public class DayActivity
{
    public string Day { get; set; } = "";
    public int Value { get; set; }
    public int Max { get; set; }
    public double Percent => Max > 0 ? ((double)Value / Max * 100) : 0;
}