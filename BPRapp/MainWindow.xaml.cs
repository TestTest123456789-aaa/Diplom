using BPRapp.Classes;
using System.Windows;

namespace BPRapp
{
    public partial class MainWindow : Window
    {
        public static MainWindow init;

        public MainWindow()
        {
            InitializeComponent();
            init = this;

            // Запускаем сервис ежедневных уведомлений
            TimerService.StartDailyNotificationCheck();

            frame.Navigate(new Pages.Authorization());
        }

        protected override void OnClosed(System.EventArgs e)
        {
            // Останавливаем сервис при закрытии приложения
            TimerService.Stop();
            base.OnClosed(e);
        }
    }
}