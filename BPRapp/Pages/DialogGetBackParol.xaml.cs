using BPRapp.Classes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages
{
    public partial class DialogGetBackParol : Page
    {
        private readonly string _email;

        // Конструктор принимает email, который передала предыдущая страница
        public DialogGetBackParol(string email)
        {
            InitializeComponent(); // ⚠️ Обязателен для привязки XAML к C#
            _email = email;
        }

        private void SendKode(object sender, RoutedEventArgs e)
        {
            string code = kod_vosstanovleniaTB.Text.Trim();

            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                MessageBox.Show("Введите корректный 6-значный код", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем код через EmailService
            if (EmailService.ValidateCode(_email, code))
            {
                EmailService.MarkCodeAsUsed(_email, code);
                // Переходим на страницу смены пароля, передавая email
                MainWindow.init.frame.Navigate(new NewParol(_email));
            }
            else
            {
                MessageBox.Show("Неверный или истёкший код", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new GetBackParol());
        }
    }
}