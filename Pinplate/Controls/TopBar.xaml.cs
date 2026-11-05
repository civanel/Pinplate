using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pinplate.Controls
{
    public partial class TopBar : UserControl
    {
        public static readonly DependencyProperty SectionTitleProperty =
            DependencyProperty.Register("SectionTitle", typeof(string), typeof(TopBar), new PropertyMetadata(string.Empty));

        public string SectionTitle
        {
            get { return (string)GetValue(SectionTitleProperty); }
            set { SetValue(SectionTitleProperty, value); }
        }

        public TopBar()
        {
            InitializeComponent();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.Trim();
            var mainWindow = Window.GetWindow(this);

            // Ищем MainViewModel (Модератор)
            if (mainWindow?.DataContext is Pinplate.ViewModels.MainViewModel mainVm)
            {
                mainVm.GlobalSearch(query);
            }
            // Ищем ExpertDashboardViewModel (Эксперт)
            else if (mainWindow?.DataContext is Pinplate.ViewModels.Expert.ExpertDashboardViewModel expertVm)
            {
                expertVm.GlobalSearch(query);
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchPlaceholder != null) SearchPlaceholder.Visibility = Visibility.Collapsed;

            // Безопасно проверяем историю (чтобы не было ошибок)
            if (HistoryPopup != null) HistoryPopup.IsOpen = true;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text) && SearchPlaceholder != null)
                SearchPlaceholder.Visibility = Visibility.Visible;

            if (HistoryPopup != null) HistoryPopup.IsOpen = false;
        }

        private void HistoryItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string query)
            {
                SearchBox.Text = query;
                if (HistoryPopup != null) HistoryPopup.IsOpen = false;
            }
        }

        private void Notification_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("У вас нет новых уведомлений.", "Уведомления");
        }

        private void Avatar_Click(object sender, RoutedEventArgs e)
        {
            // Твой старый код аватарки...
            var profileWindow = new Window
            {
                Title = "Профиль пользователя",
                Width = 300,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.White
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };
            var avatarBorder = new Border
            {
                Width = 150,
                Height = 150,
                CornerRadius = new CornerRadius(75),
                Background = Brushes.LightCoral,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };
            avatarBorder.Child = new TextBlock
            {
                Text = "M",
                FontSize = 64,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            stackPanel.Children.Add(avatarBorder);
            stackPanel.Children.Add(new TextBlock { Text = "@moderator", FontSize = 18, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center });
            stackPanel.Children.Add(new TextBlock { Text = "mod@pinplate.app", FontSize = 14, Foreground = Brushes.Gray, Margin = new Thickness(0, 5, 0, 0), HorizontalAlignment = HorizontalAlignment.Center });

            profileWindow.Content = stackPanel;
            profileWindow.ShowDialog();
        }
    }
}