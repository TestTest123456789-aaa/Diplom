using System;
using System.Configuration;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace BPRapp.Classes
{
    public static class EmailNotificationService
    {
        private static string GetNotificationSetting(string teacherEmail)
        {
            try
            {
                var connection = Connection.OpenConnection();
                string sql = "SELECT EmailNotifications FROM teachers WHERE Email = @Email";
                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Email", teacherEmail);

                object result = cmd.ExecuteScalar();
                Connection.CloseConnection(connection);

                return result?.ToString() ?? "1"; // По умолчанию включено
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка получения настроек уведомлений: {ex.Message}");
                return "1";
            }
        }

        public static bool IsNotificationsEnabled(string teacherEmail)
        {
            return GetNotificationSetting(teacherEmail) == "1";
        }

        public static void SetNotificationSetting(string teacherEmail, bool enabled)
        {
            try
            {
                var connection = Connection.OpenConnection();
                string sql = "UPDATE teachers SET EmailNotifications = @Enabled WHERE Email = @Email";
                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@Enabled", enabled ? "1" : "0");
                cmd.Parameters.AddWithValue("@Email", teacherEmail);
                cmd.ExecuteNonQuery();
                Connection.CloseConnection(connection);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка обновления настроек уведомлений: {ex.Message}");
            }
        }

        public static void SendDailyNotifications()
        {
            try
            {
                Debug.WriteLine("🔔 Запуск проверки ежедневных уведомлений...");

                // Получаем ВПР на завтра
                string tomorrow = DateTime.Now.AddDays(1).ToString("dd.MM.yyyy");

                var connection = Connection.OpenConnection();
                string sql = @"
                    SELECT b.Id, b.Date, b.Time, b.Responsible_user, b.Lesson, b.GroupId,
                           t.Email, t.FIO as TeacherFIO, l.Lesson as LessonName, g.Name as GroupName
                    FROM bpr_info b
                    INNER JOIN teachers t ON b.Responsible_user = t.Id
                    LEFT JOIN lessons l ON b.Lesson = l.Id
                    LEFT JOIN groups g ON b.GroupId = g.Id
                    WHERE b.Date = @TomorrowDate AND t.Email IS NOT NULL AND t.Email != ''";

                var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@TomorrowDate", tomorrow);

                var reader = cmd.ExecuteReader();
                int notificationsSent = 0;

                while (reader.Read())
                {
                    string email = reader["Email"].ToString();
                    string teacherFIO = reader["TeacherFIO"].ToString();
                    string lessonName = reader["LessonName"].ToString();
                    string groupName = reader["GroupName"].ToString();
                    string time = reader["Time"].ToString();

                    // Проверяем, включены ли уведомления для этого преподавателя
                    if (!IsNotificationsEnabled(email))
                    {
                        Debug.WriteLine($"️ Уведомления отключены для {teacherFIO} ({email})");
                        continue;
                    }

                    // Формируем текст письма
                    string subject = $"📋 Напоминание: ВПР завтра ({tomorrow})";
                    string body = $@"Здравствуйте, {teacherFIO}!

Напоминаем, что завтра ({tomorrow}) у вас запланирована ВПР:

📚 Предмет: {lessonName}
👥 Группа: {groupName}
⏰ Время: {time}

Пожалуйста, убедитесь, что всё готово к проведению ВПР.

С уважением,
Система управления ВПР
КГПАОУ Авиационный техникум имени А.Д. Швецова";

                    // Отправляем письмо
                    if (EmailService.SendRecoveryCode(email, "")) // Используем существующий метод
                    {
                        // Отправляем через SMTP напрямую
                        if (SendEmail(email, subject, body))
                        {
                            notificationsSent++;
                            Debug.WriteLine($"✅ Уведомление отправлено: {teacherFIO} ({email})");
                        }
                    }
                }

                reader.Close();
                Connection.CloseConnection(connection);

                Debug.WriteLine($"📊 Всего отправлено уведомлений: {notificationsSent}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка при отправке ежедневных уведомлений: {ex.Message}");
            }
        }

        private static bool SendEmail(string to, string subject, string body)
        {
            try
            {
                var smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out var port) ? port : 587;
                var senderEmail = ConfigurationManager.AppSettings["SenderEmail"];
                var senderPassword = ConfigurationManager.AppSettings["SenderPassword"].Replace(" ", "").Trim();
                var enableSsl = bool.TryParse(ConfigurationManager.AppSettings["EnableSsl"], out var ssl) && ssl;

                var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = enableSsl,
                    Timeout = 30000,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                var mail = new System.Net.Mail.MailMessage(senderEmail, to)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка отправки email: {ex.Message}");
                return false;
            }
        }
    }
}