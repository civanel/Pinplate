using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pinplate.Services;

namespace Pinplate.Controls;

public partial class ExpertSidebarMenu : UserControl
{
    public ExpertSidebarMenu()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty NavigateCommandProperty =
        DependencyProperty.Register(nameof(NavigateCommand), typeof(ICommand), typeof(ExpertSidebarMenu));

    public ICommand? NavigateCommand
    {
        get => (ICommand?)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public static readonly DependencyProperty ActiveItemProperty =
        DependencyProperty.Register(nameof(ActiveItem), typeof(string), typeof(ExpertSidebarMenu), new PropertyMetadata(string.Empty));

    public string ActiveItem
    {
        get => (string)GetValue(ActiveItemProperty);
        set => SetValue(ActiveItemProperty, value);
    }

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