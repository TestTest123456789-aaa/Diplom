using BPRapp.Classes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages
{
    public partial class NotificationSettings : Page
    {
        private readonly string _userEmail;

        public NotificationSettings(string userEmail)
        {
            InitializeComponent();
            _userEmail = userEmail;
            Loaded += NotificationSettings_Loaded;
        }

        private void NotificationSettings_Loaded(object sender, RoutedEventArgs e)
        {
            // Загружаем текущее состояние уведомлений
            if (!string.IsNullOrEmpty(_userEmail))
            {
                bool isEnabled = EmailNotificationService.IsNotificationsEnabled(_userEmail);
                NotificationToggle.IsChecked = isEnabled;
                UpdateStatusText(isEnabled);
            }
        }

        private void NotificationToggle_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем текст статуса сразу при переключении
            bool isEnabled = NotificationToggle.IsChecked == true;
            UpdateStatusText(isEnabled);
        }

        private void UpdateStatusText(bool enabled)
        {
            NotificationStatusText.Text = enabled ? "✅ Уведомления включены" : "⏸ Уведомления отключены";
            NotificationStatusText.Foreground = enabled ?
                System.Windows.Media.Brushes.ForestGreen :
                System.Windows.Media.Brushes.OrangeRed;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_userEmail))
            {
                MessageBox.Show("❌ Email пользователя не указан", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isEnabled = NotificationToggle.IsChecked == true;
            EmailNotificationService.SetNotificationSetting(_userEmail, isEnabled);

            MessageBox.Show(
                $"✅ Настройки сохранены!\n" +
                $"Email-уведомления: {(isEnabled ? "ВКЛЮЧЕНЫ" : "ОТКЛЮЧЕНЫ")}",
                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Возврат в главное меню
            if (Classes.CurrentUser.Role == "Преподаватель")
                MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.MainMenuTeachers());
            else
                MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.MainMenuAdmin());
        }

        private void CancelSettings(object sender, RoutedEventArgs e)
        {
            // Возврат без сохранения
            if (Classes.CurrentUser.Role == "Преподаватель")
                MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.MainMenuTeachers());
            else
                MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.MainMenuAdmin());
        }
    }
}