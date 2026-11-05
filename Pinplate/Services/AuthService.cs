using Microsoft.EntityFrameworkCore;
using RecipesDB;

namespace Pinplate.Services;

public class AuthService : IAuthService
{
    private readonly DBContext _db;

    public AuthService(DBContext db) => _db = db;

    public async Task<Users?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct)
            .ConfigureAwait(false);

        if (user is null) return null;

        return PasswordHelper.VerifyPassword(password, user.PasswordHash) ? user : null;
    }

    public async Task<bool> RequestPasswordResetAsync(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct)
            .ConfigureAwait(false);

        // Не палим, есть ли email — всегда true
        if (user is null) return true;

        // В вашей схеме нет поля для кода — используем App_Settings
        var code = Random.Shared.Next(100000, 999999).ToString();
        var key = $"reset:{user.UserId}";

        var setting = await _db.AppSettings
            .FirstOrDefaultAsync(s => s.SettingKey == key, ct)
            .ConfigureAwait(false);

        if (setting is null)
        {
            _db.AppSettings.Add(new App_Settings
            {
                SettingKey = key,
                SettingValue = $"{code}|{DateTime.UtcNow.AddMinutes(15):O}",
                Description = "Password reset code"
            });
        }
        else
        {
            setting.SettingValue = $"{code}|{DateTime.UtcNow.AddMinutes(15):O}";
        }

        await _db.SaveChangesAsync(ct).ConfigureAwait(false);

        // TODO: отправить code на email через IEmailService
        return true;
    }

    public async Task<bool> ConfirmPasswordResetAsync(string email, string code, string newPassword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code) || newPassword.Length < 8)
            return false;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct)
            .ConfigureAwait(false);
        if (user is null) return false;

        var key = $"reset:{user.UserId}";
        var setting = await _db.AppSettings
            .FirstOrDefaultAsync(s => s.SettingKey == key, ct)
            .ConfigureAwait(false);
        if (setting is null) return false;

        var parts = setting.SettingValue.Split('|');
        if (parts.Length != 2) return false;
        if (parts[0] != code) return false;
        if (!DateTime.TryParse(parts[1], out var expires) || expires < DateTime.UtcNow) return false;

        user.PasswordHash = PasswordHelper.HashingPassword(newPassword);
        _db.AppSettings.Remove(setting);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
        return true;
    }
}