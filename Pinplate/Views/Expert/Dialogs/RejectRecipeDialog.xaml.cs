using System.Windows;

namespace Pinplate.Views.Expert.Dialogs;

public partial class RejectRecipeDialog : Window
{
    public string RejectReason { get; private set; } = "Нарушение правил";

    public RejectRecipeDialog()
    {
        InitializeComponent();
    }

    private void OtherRadio_Checked(object sender, RoutedEventArgs e)
    {
        if (OtherReasonBox != null) OtherReasonBox.IsEnabled = true;
    }

    // Этот метод теперь используется и для крестика, и для кнопки "Отмена"
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Reject_Click(object sender, RoutedEventArgs e)
    {
        if (OtherRadio.IsChecked == true && !string.IsNullOrWhiteSpace(OtherReasonBox.Text))
        {
            RejectReason = OtherReasonBox.Text;
        }

        DialogResult = true;
        Close();
    }
}