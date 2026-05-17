using System;
using System.Timers;
using System.Diagnostics;

namespace BPRapp.Classes
{
    public static class TimerService
    {
        private static System.Timers.Timer _dailyTimer;

        public static void StartDailyNotificationCheck()
        {
            // Настраиваем таймер на проверку каждый день в 9:00
            _dailyTimer = new System.Timers.Timer();
            _dailyTimer.Interval = TimeSpan.FromHours(24).TotalMilliseconds;
            _dailyTimer.Elapsed += OnDailyCheckElapsed;
            _dailyTimer.AutoReset = true;
            _dailyTimer.Start();

            Debug.WriteLine("⏰ Сервис ежедневных уведомлений запущен");

            // Также запускаем проверку сразу при старте
            CheckAndSendNotifications();
        }

        private static void OnDailyCheckElapsed(object sender, ElapsedEventArgs e)
        {
            CheckAndSendNotifications();
        }

        private static void CheckAndSendNotifications()
        {
            // Проверяем, не слишком ли рано (ждем до 9:00)
            var now = DateTime.Now;
            if (now.Hour >= 9)
            {
                EmailNotificationService.SendDailyNotifications();
            }
        }

        public static void Stop()
        {
            _dailyTimer?.Stop();
            _dailyTimer?.Dispose();
            Debug.WriteLine("⏹️ Сервис ежедневных уведомлений остановлен");
        }
    }
}