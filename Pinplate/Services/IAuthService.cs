using RecipesDB;

namespace Pinplate.Services;

public interface IAuthService
{
    Task<Users?> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<bool> RequestPasswordResetAsync(string email, CancellationToken ct = default);
    Task<bool> ConfirmPasswordResetAsync(string email, string code, string newPassword, CancellationToken ct = default);
}