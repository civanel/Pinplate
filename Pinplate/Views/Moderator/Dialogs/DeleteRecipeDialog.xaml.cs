using System.Windows;

namespace Pinplate.Views.Moderator.Dialogs
{
    public partial class DeleteRecipeDialog : Window
    {
        public DeleteRecipeDialog() => InitializeComponent();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
        private void Delete_Click(object sender, RoutedEventArgs e) => Close();
    }
}