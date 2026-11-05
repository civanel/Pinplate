namespace Pinplate.Services;

public record ChartPoint(DateTime Date, int Value);
public record TopRecipe(int Id, string Title, int Reviews);
public record TopUser(int Id, string UserName, int RecipesCount);
public record ActivityDay(string Day, int Value, double Percent);

public interface IStatisticsService
{
    Task<int> GetReportsCountAsync(CancellationToken ct = default);
    Task<int> GetSpamCountAsync(CancellationToken ct = default);
    Task<int> GetOnModerationCountAsync(CancellationToken ct = default);
    Task<int> GetUsersCountAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ChartPoint>> GetChartPointsAsync(string period, CancellationToken ct = default);
    Task<IReadOnlyList<TopRecipe>> GetTopRecipesAsync(int take = 5, CancellationToken ct = default);
    Task<IReadOnlyList<TopUser>> GetTopUsersAsync(int take = 5, CancellationToken ct = default);
    Task<IReadOnlyList<ActivityDay>> GetActivityByDayAsync(CancellationToken ct = default);
    Task ExportCsvAsync(string filePath, string period, CancellationToken ct = default);
}