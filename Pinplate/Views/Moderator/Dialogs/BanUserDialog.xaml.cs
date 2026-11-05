using System;
using System.Linq;
using System.Windows;
using RecipesDB;

namespace Pinplate.Views.Moderator.Dialogs
{
    public partial class BanUserDialog : Window
    {
        public int TargetUserId { get; set; }
        public string TargetUserName { get; set; } = "";

        public BanUserDialog(int userId, string userName)
        {
            InitializeComponent();
            TargetUserId = userId;
            TargetUserName = userName;

            var nameText = this.FindName("UserNameText") as System.Windows.Controls.TextBlock;
            if (nameText != null) nameText.Text = $"@{TargetUserName}";
        }

        // Этот метод закрывает окно (для крестика и кнопки "Отмена")
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Ban_Click(object sender, RoutedEventArgs e)
        {
            string reason = "Нарушение правил";
            if (OtherRadio.IsChecked == true && !string.IsNullOrWhiteSpace(OtherReasonBox.Text))
                reason = OtherReasonBox.Text;
            else if (this.FindName("SpamRadio") is System.Windows.Controls.RadioButton spam && spam.IsChecked == true)
                reason = "Спам";

            using (var db = new DBContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == TargetUserId);
                if (user != null)
                {
                    user.IsBanned = true;
                    user.BanReason = reason;
                    db.SaveChanges();
                }
            }

            MessageBox.Show($"Пользователь @{TargetUserName} забанен.\nПричина: {reason}", "Успех");
            DialogResult = true;
            Close();
        }

        private void OtherRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (OtherReasonBox != null) OtherReasonBox.IsEnabled = true;
        }
    }
}