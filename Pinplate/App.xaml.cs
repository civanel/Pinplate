using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pinplate.Services;
using Pinplate.Views;
using Pinplate.Views.Main;
using RecipesDB;
using System;
using System.Linq;
using System.Windows;

namespace Pinplate
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var sc = new ServiceCollection();
            sc.AddTransient<DBContext>();
            sc.AddScoped<IAuthService, AuthService>();
            sc.AddScoped<IUserService, UserService>();
            sc.AddScoped<IRecipeService, RecipeService>();
            sc.AddScoped<ILogService, LogService>();
            sc.AddScoped<IStatisticsService, StatisticsService>();
            sc.AddSingleton<IEmailService, EmailService>();

            sc.AddTransient<LoginWindow>();
            sc.AddTransient<MainWindow>();

            Services = sc.BuildServiceProvider();

            // 1. СНАЧАЛА создаем тестового юзера (если база пустая)
            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DBContext>();

                if (!db.Users.Any())
                {
                    var admin = new Users
                    {
                        Name = "Админ",
                        Email = "admin@mail.ru",
                        PasswordHash = PasswordHelper.HashingPassword("123456"),
                        Role = "Moderator",
                        CreatedAt = DateTime.UtcNow
                    };
                    db.Users.Add(admin);
                    db.SaveChanges();
                }
            }

            // 2. И ТОЛЬКО ПОТОМ показываем окно входа
            var login = Services.GetRequiredService<LoginWindow>();
            login.Show();
        }
    }
}