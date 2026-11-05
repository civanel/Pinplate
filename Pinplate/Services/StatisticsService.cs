using Microsoft.EntityFrameworkCore;
using RecipesDB;

namespace Pinplate.Services;

public class StatisticsService : IStatisticsService
{
    private readonly DBContext _db;

    public StatisticsService(DBContext db) => _db = db;

    public Task<int> GetReportsCountAsync(CancellationToken ct = default)
        => _db.Complaints.CountAsync(c => c.Status == "Открыта", ct);

    public Task<int> GetSpamCountAsync(CancellationToken ct = default)
        => _db.Complaints.CountAsync(c => c.Reason == "Спам", ct);

    // В Recipes нет Status — «на модерации» считаем как «создан за последние 7 дней и без отзывов»
    // Если добавите Status — замените на CountAsync(r => r.Status == "Pending")
    public Task<int> GetOnModerationCountAsync(CancellationToken ct = default)
        => _db.Recipes.CountAsync(r => r.CreatedAt >= DateTime.UtcNow.AddDays(-7), ct);

    public Task<int> GetUsersCountAsync(CancellationToken ct = default)
        => _db.Users.CountAsync(ct);

    public async Task<IReadOnlyList<ChartPoint>> GetChartPointsAsync(string period, CancellationToken ct = default)
    {
        var days = period switch { "Неделя" => 7, "Месяц" => 30, "Год" => 365, _ => 7 };
        var from = DateTime.UtcNow.AddDays(-days);

        var raw = await _db.Log_Entries.AsNoTracking()
            .Where(l => l.CreatedAt >= from)
            .GroupBy(l => l.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Value = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(ct).ConfigureAwait(false);

        return raw.Select(x => new ChartPoint(x.Date, x.Value)).ToList();
    }

    // В Recipes нет Rating — считаем по количеству отзывов
    public async Task<IReadOnlyList<TopRecipe>> GetTopRecipesAsync(int take = 5, CancellationToken ct = default)
    {
        return await _db.Recipes.AsNoTracking()
            .Select(r => new TopRecipe(r.RecipeId, r.Name, r.RecipeReviews.Count))
            .OrderByDescending(x => x.Reviews)
            .Take(take)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TopUser>> GetTopUsersAsync(int take = 5, CancellationToken ct = default)
    {
        return await _db.Users.AsNoTracking()
            .Select(u => new TopUser(
                u.UserId,
                u.Name,
                u.RecipeReviews.Count)) // у Users нет прямой связи с Recipes, считаем по отзывам
            .OrderByDescending(u => u.RecipesCount)
            .Take(take)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ActivityDay>> GetActivityByDayAsync(CancellationToken ct = default)
    {
        var from = DateTime.UtcNow.Date.AddDays(-6);
        var raw = await _db.Log_Entries.AsNoTracking()
            .Where(l => l.CreatedAt >= from)
            .GroupBy(l => l.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Value = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(ct).ConfigureAwait(false);

        var max = raw.Count == 0 ? 1 : raw.Max(x => x.Value);
        return raw.Select(x => new ActivityDay(
            x.Date.ToString("dd.MM"),
            x.Value,
            Math.Round(100.0 * x.Value / max, 1)
        )).ToList();
    }

    public async Task ExportCsvAsync(string filePath, string period, CancellationToken ct = default)
    {
        var points = await GetChartPointsAsync(period, ct).ConfigureAwait(false);
        var lines = new List<string> { "Дата;Количество" };
        lines.AddRange(points.Select(p => $"{p.Date:yyyy-MM-dd};{p.Value}"));
        await System.IO.File.WriteAllLinesAsync(filePath, lines, ct).ConfigureAwait(false);
    }
}