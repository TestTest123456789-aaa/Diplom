using System.Net;
using System.Net.Mail;
using System;
using System.Configuration;
using MySql.Data.MySqlClient;

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

        private static bool EnableSsl =>
            bool.TryParse(ConfigurationManager.AppSettings["EnableSsl"], out var ssl) && ssl;

        public static bool SendRecoveryCode(string recipientEmail, string code)
        {
            try
            {
                // Проверка настроек
                if (string.IsNullOrEmpty(SenderEmail))
                {
                    System.Diagnostics.Debug.WriteLine("❌ Ошибка: SenderEmail не задан в App.config");
                    return false;
                }
                if (string.IsNullOrEmpty(SenderPassword) || SenderPassword == "xxxx xxxx xxxx xxxx" || SenderPassword.Length < 10)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Ошибка: SenderPassword не задан или недействителен в App.config");
                    System.Diagnostics.Debug.WriteLine($"   Текущее значение: '{SenderPassword}'");
                    System.Diagnostics.Debug.WriteLine("   Для Gmail создайте App Password: https://myaccount.google.com/security");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"📧 Попытка отправки письма...");
                System.Diagnostics.Debug.WriteLine($"   SMTP Server: {SmtpServer}:{SmtpPort}");
                System.Diagnostics.Debug.WriteLine($"   SSL: {EnableSsl}");
                System.Diagnostics.Debug.WriteLine($"   From: {SenderEmail}");
                System.Diagnostics.Debug.WriteLine($"   To: {recipientEmail}");

                var client = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(SenderEmail, SenderPassword),
                    EnableSsl = EnableSsl,
                    Timeout = 30000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                var mail = new MailMessage(SenderEmail, recipientEmail)
                {
                    Subject = "🔐 Восстановление пароля - Система ВПР",
                    Body = $"Здравствуйте!\n\n" +
                           $"Ваш код восстановления пароля: {code}\n" +
                           $"Код действителен в течение 15 минут.\n" +
                           $"Если вы не запрашивали восстановление, проигнорируйте это письмо.\n\n" +
                           $"С уважением, Администрация ВПР",
                    IsBodyHtml = false
                };

                client.Send(mail);
                System.Diagnostics.Debug.WriteLine($"✅ Письмо успешно отправлено на {recipientEmail}");
                return true;
            }
            catch (SmtpException smtpEx)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SMTP ошибка: {smtpEx.Message}");
                System.Diagnostics.Debug.WriteLine($"   StatusCode: {smtpEx.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"   Server: {SmtpServer}:{SmtpPort}");
                System.Diagnostics.Debug.WriteLine($"   From: {SenderEmail}");

                if (smtpEx.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   Inner Exception: {smtpEx.InnerException.Message}");
                    if (smtpEx.InnerException.InnerException != null)
                        System.Diagnostics.Debug.WriteLine($"   Inner-Inner: {smtpEx.InnerException.InnerException.Message}");
                }

                // Подсказки для частых ошибок
                if (smtpEx.StatusCode == SmtpStatusCode.AuthenticationFailed)
                {
                    System.Diagnostics.Debug.WriteLine("\n💡 ПОДСКАЗКА: Неверный пароль. Для Gmail используйте App Password, а не обычный пароль.");
                    System.Diagnostics.Debug.WriteLine("   Как создать: https://support.google.com/accounts/answer/185833");
                }
                else if (smtpEx.Message.Contains("535") || smtpEx.Message.Contains("Authentication"))
                {
                    System.Diagnostics.Debug.WriteLine("\n💡 ПОДСКАЗКА: Требуется App Password для Gmail.");
                    System.Diagnostics.Debug.WriteLine("   1. Включите 2FA в аккаунте Google");
                    System.Diagnostics.Debug.WriteLine("   2. Перейдите: https://myaccount.google.com/security");
                    System.Diagnostics.Debug.WriteLine("   3. Создайте App Password для 'Mail' и 'Windows Computer'");
                    System.Diagnostics.Debug.WriteLine("   4. Вставьте 16-значный код без пробелов в App.config");
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Общая ошибка: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"   Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"   Inner: {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                        System.Diagnostics.Debug.WriteLine($"   Inner-Inner: {ex.InnerException.InnerException.Message}");
                }
                return false;
            }
        }

        public static string GenerateCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public static bool ValidateCode(string email, string code)
        {
            var connection = Connection.OpenConnection();
            string sql = "SELECT COUNT(*) FROM password_recovery WHERE Email=@Email AND RecoveryCode=@Code AND Used=0 AND CreatedAt >= DATE_SUB(NOW(), INTERVAL 15 MINUTE)";
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Code", code);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            Connection.CloseConnection(connection);
            return count > 0;
        }

        public static void MarkCodeAsUsed(string email, string code)
        {
            var connection = Connection.OpenConnection();
            string sql = "UPDATE password_recovery SET Used=1 WHERE Email=@Email AND RecoveryCode=@Code";
            var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.ExecuteNonQuery();
            Connection.CloseConnection(connection);
        }
    }
}