using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BPRapp.Classes
{
    public static class EmailService
    {
        private static string SmtpServer =>
            ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";

        private static int SmtpPort =>
            int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out var port) ? port : 587;

        private static string SenderEmail =>
            ConfigurationManager.AppSettings["SenderEmail"] ?? "";

        private static string SenderPassword =>
            ConfigurationManager.AppSettings["SenderPassword"] ?? "";

        public static bool SendRecoveryCode(string recipientEmail, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(SenderEmail))
                {
                    Debug.WriteLine("❌ Ошибка: SenderEmail не задан");
                    return false;
                }

                string cleanPassword = SenderPassword.Replace(" ", "").Trim();

                if (string.IsNullOrEmpty(cleanPassword) || cleanPassword.Length < 10)
                {
                    Debug.WriteLine("❌ Ошибка: SenderPassword слишком короткий");
                    return false;
                }

                Debug.WriteLine($"📧 Отправка через MailKit...");
                Debug.WriteLine($"   SMTP: {SmtpServer}:{SmtpPort}");
                Debug.WriteLine($"   From: {SenderEmail}");
                Debug.WriteLine($"   To: {recipientEmail}");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Система ВПР", SenderEmail));
                message.To.Add(new MailboxAddress("", recipientEmail));
                message.Subject = "🔐 Восстановление пароля - Система ВПР";
                message.Body = new TextPart("plain")
                {
                    Text = $"Здравствуйте!\n\n" +
                           $"Ваш код восстановления пароля: {code}\n" +
                           $"Код действителен в течение 15 минут.\n\n" +
                           $"С уважением, Администрация ВПР"
                };

                using (var client = new SmtpClient())
                {
                    // Для порта 587 используем StartTls
                    client.Connect(SmtpServer, SmtpPort, SecureSocketOptions.StartTls);

                    // Аутентификация
                    client.Authenticate(SenderEmail, cleanPassword);

                    // Отправка
                    client.Send(message);

                    // Отключение
                    client.Disconnect(true);
                }

                Debug.WriteLine($"✅ Письмо успешно отправлено!");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n❌ ОШИБКА: {ex.Message}");
                if (ex.InnerException != null)
                    Debug.WriteLine($"   Inner: {ex.InnerException.Message}");
                return false;
            }
        }

        public static string GenerateCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public static bool ValidateCode(string email, string code)
        {
            var connection = Connection.OpenConnection();
            string sql = "SELECT COUNT(*) FROM password_recovery WHERE Email=@Email AND RecoveryCode=@Code AND Used=0 AND CreatedAt >= DATE_SUB(NOW(), INTERVAL 15 MINUTE)";
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Code", code);

            int count = 0;
            try
            {
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    count = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            finally
            {
                Connection.CloseConnection(connection);
            }

            return count > 0;
        }

        public static void MarkCodeAsUsed(string email, string code)
        {
            var connection = Connection.OpenConnection();
            string sql = "UPDATE password_recovery SET Used=1 WHERE Email=@Email AND RecoveryCode=@Code";
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Code", code);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка БД: {ex.Message}");
            }
            finally
            {
                Connection.CloseConnection(connection);
            }
        }
    }
}