using Microsoft.EntityFrameworkCore;
using RecipesDB;

namespace Pinplate.Services;

public class LogService : ILogService
{
    private readonly DBContext _db;

    public LogService(DBContext db) => _db = db;

    public async Task<IReadOnlyList<Log_Entries>> GetAllAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        => await _db.Log_Entries.AsNoTracking()
            .OrderByDescending(l => l.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct).ConfigureAwait(false);

    public Task<int> CountAsync(CancellationToken ct = default)
        => _db.Log_Entries.CountAsync(ct);

    public async Task WriteAsync(string userName, string action, string status, string targetTitle = "", CancellationToken ct = default)
    {
        _db.Log_Entries.Add(new Log_Entries
        {
            CreatedAt = DateTime.UtcNow,
            UserName = userName,
            Action = action,
            Status = status,
            TargetTitle = targetTitle
        });
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}