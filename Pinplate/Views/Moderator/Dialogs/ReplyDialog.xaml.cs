using System;
using System.Linq;
using System.Windows;
using RecipesDB;

namespace Pinplate.Views.Moderator.Dialogs
{
    public partial class ReplyDialog : Window
    {
        public int ComplaintId { get; set; }
        public string TargetUser { get; set; } = "";
        public string ComplaintTitle { get; set; } = "";

        public ReplyDialog(int complaintId, string targetUser, string title)
        {
            InitializeComponent();
            ComplaintId = complaintId;
            TargetUser = targetUser;
            ComplaintTitle = title;

            var userText = this.FindName("TargetUserText") as System.Windows.Controls.TextBlock;
            if (userText != null) userText.Text = $"Кому: @{TargetUser}";
        }

        // Этот метод закрывает окно (для крестика и кнопки "Отмена")
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TB_Message.Text))
            {
                MessageBox.Show("Введите текст ответа!", "Ошибка");
                return;
            }

            using (var db = new DBContext())
            {
                var newReply = new Replies
                {
                    ComplaintId = ComplaintId,
                    RecipientName = TargetUser,
                    ComplaintTitle = ComplaintTitle,
                    Message = TB_Message.Text,
                    AuthorName = "Модератор",
                    CreatedAt = DateTime.UtcNow
                };
                db.Replies.Add(newReply);

                var complaint = db.Complaints.FirstOrDefault(c => c.ComplaintId == ComplaintId);
                if (complaint != null) complaint.Status = "Решено";

                db.SaveChanges();
            }

            MessageBox.Show("Ответ успешно отправлен!", "Успех");
            DialogResult = true;
            Close();
        }
    }
}