using System.Diagnostics;

namespace Pinplate.Services;

public class EmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        // Заглушка: пишем в Debug, чтобы не настраивать SMTP на этапе разработки.
        // В проде — MailKit/SendGrid/ваш SMTP.
        Debug.WriteLine($"[EMAIL] to={to} | {subject} | {body}");
        return Task.CompletedTask;
    }
}