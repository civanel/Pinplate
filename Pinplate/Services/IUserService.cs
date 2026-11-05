using RecipesDB;

namespace Pinplate.Services;

public interface IUserService
{
    Task<IReadOnlyList<Users>> GetAllAsync(CancellationToken ct = default);
    Task<Users?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Users>> SearchAsync(string query, CancellationToken ct = default);
    Task DeleteAsync(int userId, CancellationToken ct = default);
    Task UpdateProfileAsync(int userId, string name, string email, CancellationToken ct = default);
}