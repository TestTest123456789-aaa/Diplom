using System.Windows;
using System.Windows.Controls;
using System.Linq;
using BPRapp.Classes;

namespace BPRapp.Pages
{
    public partial class GetBackParol : Page
    {
        private string _userEmail;

        public GetBackParol()
        {
            InitializeComponent();
        }

        private void SendEmail(object sender, RoutedEventArgs e)
        {
            string email = emailTB.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                MessageBox.Show("Введите корректный email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем, есть ли пользователь с такой почтой
            var user = Users.Select()
                .FirstOrDefault(u => u.Email?.ToLower() == email);

            if (user == null)
            {
                MessageBox.Show("Пользователь с такой почтой не найден", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Генерируем и сохраняем код
            string code = EmailService.GenerateCode();
            SaveRecoveryCode(email, code);

            // Отправляем письмо
            if (EmailService.SendRecoveryCode(email, code))
            {
                MessageBox.Show($"Код восстановления отправлен на {email}", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                // ← Передаём email в DialogGetBackParol
                MainWindow.init.frame.Navigate(new DialogGetBackParol(email));
            }
            else
            {
                MessageBox.Show("Ошибка отправки письма. Проверьте настройки SMTP.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveRecoveryCode(string email, string code)
        {
            var connection = Classes.Connection.OpenConnection();
            string sql = "INSERT INTO password_recovery (Email, RecoveryCode) VALUES (@Email, @Code)";
            var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.ExecuteNonQuery();
            Classes.BPR_info.CloseConnection(connection);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        }
    }
}