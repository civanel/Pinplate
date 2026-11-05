using System.Windows;
using System.Windows.Controls;

namespace Pinplate.Views.Expert;

public partial class TopView : UserControl
{
    public TopView()
    {
        InitializeComponent();
    }

    private void Recipe_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Pinplate.ViewModels.Expert.TopRecipeItem recipe)
        {
            var window = new Window
            {
                Title = recipe.Title,
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = System.Windows.Media.Brushes.White,
                WindowStyle = WindowStyle.ToolWindow
            };

            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = recipe.Title, FontSize = 24, FontWeight = FontWeights.Bold });
            panel.Children.Add(new TextBlock { Text = $"Автор: {recipe.Author}", FontSize = 14, Margin = new Thickness(0, 10, 0, 0) });
            panel.Children.Add(new TextBlock { Text = $"Рейтинг: {recipe.Rating:F1} ★", FontSize = 14, Foreground = System.Windows.Media.Brushes.Goldenrod, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 20) });
            panel.Children.Add(new TextBlock { Text = "Описание:", FontSize = 14, FontWeight = FontWeights.SemiBold });
            panel.Children.Add(new TextBlock { Text = recipe.Description, FontSize = 14, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 5, 0, 0) });

            window.Content = panel;
            window.ShowDialog();
        }
    }
}