using RecipesDB;

namespace Pinplate.Services;

public interface ILogService
{
    Task<IReadOnlyList<Log_Entries>> GetAllAsync(int skip = 0, int take = 50, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task WriteAsync(string userName, string action, string status, string targetTitle = "", CancellationToken ct = default);
}