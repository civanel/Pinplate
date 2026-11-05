using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pinplate.Services;

namespace Pinplate.Controls;

public partial class SidebarMenu : UserControl
{
    public SidebarMenu()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty NavigateCommandProperty =
        DependencyProperty.Register(nameof(NavigateCommand), typeof(ICommand), typeof(SidebarMenu));

    public ICommand? NavigateCommand
    {
        get => (ICommand?)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public static readonly DependencyProperty ActiveItemProperty =
        DependencyProperty.Register(nameof(ActiveItem), typeof(string), typeof(SidebarMenu), new PropertyMetadata(string.Empty));

    public string ActiveItem
    {
        get => (string)GetValue(ActiveItemProperty);
        set => SetValue(ActiveItemProperty, value);
    }

    // МЕТОД ВЫХОДА
    private void Logout_Click(object sender, MouseButtonEventArgs e)
    {
        var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            var authService = App.Services.GetService(typeof(IAuthService)) as IAuthService;
            var loginWindow = new Views.LoginWindow(authService!);
            loginWindow.Show();

            Window.GetWindow(this)?.Close();
        }
    }
}