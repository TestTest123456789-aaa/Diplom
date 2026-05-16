using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPRapp.Pages
{
    public partial class ChangePassword : Page
    {
        private readonly string _email;

        public ChangePassword(string email)
        {
            InitializeComponent();
            _email = email;
        }

        private bool ValidatePassword(string password, out string error)
        {
            error = "";

            if (string.IsNullOrEmpty(password))
            {
                error = "Пароль не может быть пустым";
                return false;
            }
            if (password.Length < 8)
            {
                error = "Пароль должен содержать минимум 8 символов";
                return false;
            }
            if (!Regex.IsMatch(password, @"[A-Za-z]") || !Regex.IsMatch(password, @"[0-9]"))
            {
                error = "Пароль должен содержать буквы и цифры";
                return false;
            }
            return true;
        }

        private void SavePassword(object sender, RoutedEventArgs e)
        {
            string newPassword = newPasswordTB.Password;
            string confirmPassword = confirmPasswordTB.Password;

            if (newPassword != confirmPassword)
            {
                statusText.Text = "Пароли не совпадают";
                statusText.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            if (!ValidatePassword(newPassword, out string error))
            {
                statusText.Text = error;
                statusText.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            // Находим пользователя и обновляем пароль
            var user = Users.Select().FirstOrDefault(u => u.Email?.ToLower() == _email?.ToLower());
            if (user != null)
            {
                user.Parol = newPassword;
                user.Update();

                statusText.Text = "✅ Пароль успешно изменён!";
                statusText.Foreground = System.Windows.Media.Brushes.Green;

                // Возврат на авторизацию через 2 секунды
                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    MainWindow.init.frame.Navigate(new Pages.Authorization());
                };
                timer.Start();
            }
            else
            {
                statusText.Text = "❌ Ошибка обновления пароля";
                statusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.EnterRecoveryCode(_email));
        }
    }
}