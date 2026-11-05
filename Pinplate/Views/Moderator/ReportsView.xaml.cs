using System.Windows.Controls;
using System.Windows.Input;

namespace Pinplate.Views.Moderator;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();
    }

    private void Reset_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ViewModels.ReportsViewModel vm)
            vm.ResetCommand.Execute(null);
    }
}