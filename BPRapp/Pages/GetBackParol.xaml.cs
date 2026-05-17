using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Text.RegularExpressions;
using BPRapp.Classes;
using MySql.Data.MySqlClient;

namespace BPRapp.Pages
{
    public partial class GetBackParol : Page
    {
        public GetBackParol()
        {
            InitializeComponent();
            UpdateSendButtonState();
        }

        private void EmailTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSendButtonState();
            ValidateEmail();
        }

        private void ValidateEmail()
        {
            string email = emailTB.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                emailHint.Text = "";
                emailHint.Visibility = Visibility.Collapsed;
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                emailHint.Text = "⚠️ Введите корректный email";
                emailHint.Visibility = Visibility.Visible;
            }
            else
            {
                emailHint.Text = "";
                emailHint.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateSendButtonState()
        {
            string email = emailTB.Text.Trim();
            SendButton.IsEnabled = !string.IsNullOrEmpty(email) && email.Contains("@");
            SendButton.Opacity = SendButton.IsEnabled ? 1.0 : 0.6;
        }

        private void SendEmail(object sender, RoutedEventArgs e)
        {
            string email = emailTB.Text.Trim().ToLower();

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                emailHint.Text = "❌ Неверный формат email";
                emailHint.Visibility = Visibility.Visible;
                return;
            }

            // Проверяем наличие пользователя
            var user = Users.Select().FirstOrDefault(u => u.Email?.ToLower() == email);

            if (user == null)
            {
                MessageBox.Show("Пользователь с такой почтой не найден в системе", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Генерируем и сохраняем код
            string code = EmailService.GenerateCode();
            SaveRecoveryCode(email, code);

            // Отправляем письмо
            if (EmailService.SendRecoveryCode(email, code))
            {
                MessageBox.Show($"✅ Код восстановления отправлен на {email}\n\nПроверьте папку «Входящие» и «Спам»",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.init.frame.Navigate(new DialogGetBackParol(email));
            }
            else
            {
                MessageBox.Show(
                    "❌ Ошибка отправки письма.\n\n" +
                    "Возможные причины:\n" +
                    "• Неверный App Password в настройках\n" +
                    "• Отключена двухфакторная аутентификация в Google\n" +
                    "• Проблемы с интернет-соединением",
                    "Ошибка SMTP",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SaveRecoveryCode(string email, string code)
        {
            var connection = Classes.Connection.OpenConnection();
            try
            {
                string sql = "INSERT INTO password_recovery (Email, RecoveryCode) VALUES (@Email, @Code)";
                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Code", code);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Classes.Connection.CloseConnection(connection);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        }
    }
}