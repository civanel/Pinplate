using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pinplate.Views.Moderator
{
    public partial class ModeratorDashboard : UserControl
    {
        public ModeratorDashboard()
        {
            InitializeComponent();
        }

        // Переход на вкладку "Все жалобы"
        private void OpenAllReports_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is Pinplate.ViewModels.MainViewModel mainVm)
            {
                mainVm.NavigateCommand.Execute("Reports");
            }
        }

        // Переход на вкладку "Все логи"
        private void OpenAllLogs_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this)?.DataContext is Pinplate.ViewModels.MainViewModel mainVm)
            {
                mainVm.NavigateCommand.Execute("Logs");
            }
        }
    }
}