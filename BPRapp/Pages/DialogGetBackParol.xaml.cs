using BPRapp.Classes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages
{
    public partial class DialogGetBackParol : Page
    {
        private readonly string _email;

        public DialogGetBackParol(string email)
        {
            InitializeComponent();
            _email = email;
        }

        private void SendKode(object sender, RoutedEventArgs e)
        {
            string code = kod_vosstanovleniaTB.Text.Trim();

            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                MessageBox.Show("Введите корректный 6-значный код", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EmailService.ValidateCode(_email, code))
            {
                EmailService.MarkCodeAsUsed(_email, code);
                MainWindow.init.frame.Navigate(new ChangePassword(_email));
            }
            else
            {
                MessageBox.Show("Неверный или истёкший код", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new GetBackParol());
        }
    }
}