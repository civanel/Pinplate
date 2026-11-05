using Microsoft.EntityFrameworkCore;
using RecipesDB;

namespace Pinplate.Services;

public class UserService : IUserService
{
    private readonly DBContext _db;

    public UserService(DBContext db) => _db = db;

    public async Task<IReadOnlyList<Users>> GetAllAsync(CancellationToken ct = default)
        => await _db.Users.AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(ct).ConfigureAwait(false);

    public async Task<Users?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == id, ct).ConfigureAwait(false);

    public async Task<IReadOnlyList<Users>> SearchAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return await GetAllAsync(ct).ConfigureAwait(false);

        var q = query.Trim().ToLower();
        return await _db.Users.AsNoTracking()
            .Where(u => u.Name.ToLower().Contains(q) || u.Email.ToLower().Contains(q))
            .OrderBy(u => u.Name)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int userId, CancellationToken ct = default)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId, ct).ConfigureAwait(false);
        if (u is null) return;
        _db.Users.Remove(u);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task UpdateProfileAsync(int userId, string name, string email, CancellationToken ct = default)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.UserId == userId, ct).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Пользователь {userId} не найден");
        u.Name = name;
        u.Email = email;
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}