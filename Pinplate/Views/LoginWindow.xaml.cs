using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Pinplate.Services;
using Pinplate.Views.Main;

namespace Pinplate.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IAuthService _authService;
        private bool _resetCodeSent;

        public LoginWindow(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private void TabLogin_Click(object sender, RoutedEventArgs e)
        {
            PanelLogin.Visibility = Visibility.Visible;
            PanelReset.Visibility = Visibility.Collapsed;
            MessageText.Visibility = Visibility.Collapsed;
        }

        private void TabReset_Click(object sender, RoutedEventArgs e)
        {
            PanelLogin.Visibility = Visibility.Collapsed;
            PanelReset.Visibility = Visibility.Visible;
            MessageText.Visibility = Visibility.Collapsed;
        }

        // --- ГЛАЗ (ПОКАЗАТЬ/СКРЫТЬ ПАРОЛЬ) ---
        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (PB_LoginPassword.Visibility == Visibility.Visible)
            {
                // Если пароль скрыт (звездочки) - показываем текст
                TB_LoginPasswordVisible.Text = PB_LoginPassword.Password;
                PB_LoginPassword.Visibility = Visibility.Collapsed;
                TB_LoginPasswordVisible.Visibility = Visibility.Visible;
            }
            else
            {
                // Если пароль виден - скрываем обратно в звездочки
                PB_LoginPassword.Password = TB_LoginPasswordVisible.Text;
                TB_LoginPasswordVisible.Visibility = Visibility.Collapsed;
                PB_LoginPassword.Visibility = Visibility.Visible;
            }
        }

        // --- ВХОД ---
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            MessageText.Visibility = Visibility.Collapsed;
            var email = TB_LoginEmail.Text?.Trim() ?? string.Empty;

            // Берем пароль из того поля, которое сейчас видно!
            var password = PB_LoginPassword.Visibility == Visibility.Visible
                ? PB_LoginPassword.Password
                : TB_LoginPasswordVisible.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Заполните email и пароль", true);
                return;
            }

            BtnLogin.IsEnabled = false;
            LoginProgress.Visibility = Visibility.Visible;

            try
            {
                var user = await _authService.LoginAsync(email, password);
                if (user == null)
                {
                    ShowMessage("Неверный email или пароль", true);
                    return;
                }

                // РАЗДЕЛЕНИЕ ПО РОЛЯМ
                Window nextWindow;

                if (user.Role == "Expert")
                {
                    nextWindow = new Pinplate.Views.Main.ExpertDashboard();
                }
                else if (user.Role == "Moderator")
                {
                    nextWindow = new Pinplate.Views.Main.MainWindow();
                }
                else
                {
                    MessageBox.Show($"У вас нет доступа к панели управления (роль: {user.Role}).", "Доступ запрещен");
                    return;
                }

                nextWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка: {ex.Message}", true);
            }
            finally
            {
                BtnLogin.IsEnabled = true;
                LoginProgress.Visibility = Visibility.Collapsed;
            }
        }

        // --- СБРОС ПАРОЛЯ ---
        private async void Reset_Click(object sender, RoutedEventArgs e)
        {
            MessageText.Visibility = Visibility.Collapsed;
            var email = TB_ResetEmail.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowMessage("Введите email", true);
                return;
            }

            BtnReset.IsEnabled = false;
            ResetProgress.Visibility = Visibility.Visible;

            try
            {
                if (!_resetCodeSent)
                {
                    await _authService.RequestPasswordResetAsync(email);
                    _resetCodeSent = true;
                    PanelResetStep2.Visibility = Visibility.Visible;
                    BtnReset.Content = "СОХРАНИТЬ ПАРОЛЬ";
                    ShowMessage("Если аккаунт существует, код отправлен", false);
                }
                else
                {
                    var code = TB_ResetCode.Text?.Trim() ?? string.Empty;
                    var newPwd = PB_NewPassword.Password;
                    var confirmPwd = PB_NewPasswordConfirm.Password;

                    if (newPwd != confirmPwd)
                    {
                        ShowMessage("Пароли не совпадают", true);
                        return;
                    }

                    var ok = await _authService.ConfirmPasswordResetAsync(email, code, newPwd);
                    if (!ok)
                    {
                        ShowMessage("Неверный или просроченный код", true);
                        return;
                    }

                    ShowMessage("Пароль изменён. Войдите.", false);
                    await System.Threading.Tasks.Task.Delay(1500);
                    _resetCodeSent = false;
                    PanelResetStep2.Visibility = Visibility.Collapsed;
                    TB_ResetCode.Clear();
                    PB_NewPassword.Clear();
                    PB_NewPasswordConfirm.Clear();
                    BtnReset.Content = "ОТПРАВИТЬ ССЫЛКУ";
                    TabLogin_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка: {ex.Message}", true);
            }
            finally
            {
                BtnReset.IsEnabled = true;
                ResetProgress.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowMessage(string text, bool isError)
        {
            MessageText.Text = text;
            MessageText.Foreground = isError
                ? (Brush)FindResource("AccentPinkBrush")
                : (Brush)FindResource("AccentGreenBrush");
            MessageText.Visibility = Visibility.Visible;
        }
    }
}