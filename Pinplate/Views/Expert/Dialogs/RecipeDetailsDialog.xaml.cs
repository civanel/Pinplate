using System.Windows;
using System.Windows.Input;

namespace Pinplate.Views.Expert.Dialogs;

public partial class RecipeDetailsDialog : Window
{
    public RecipeDetailsDialog()
    {
        InitializeComponent();
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Close_Click(object sender, MouseButtonEventArgs e) => Close();

    private void Approve_Click(object sender, RoutedEventArgs e)
    {
        // TODO: логика одобрения
        Close();
    }
}