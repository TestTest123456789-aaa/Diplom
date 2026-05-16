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
    public partial class NewParol : Page
    {
        private readonly string _email;  // ← Поле для хранения email

        // ← Конструктор с параметром email
        public NewParol(string email)
        {
            InitializeComponent();
            _email = email;
        }

        // ← Пустой конструктор (опционально)
        public NewParol() : this("")
        {
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

        private void CreateParol(object sender, RoutedEventArgs e)
        {
            string newPassword = parolTB.Text;
            string confirmPassword = confirm_parolTB.Text;

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidatePassword(newPassword, out string error))
            {
                MessageBox.Show(error, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ← Находим пользователя по email и обновляем пароль
            var user = Users.Select().FirstOrDefault(u => u.Email?.ToLower() == _email?.ToLower());
            if (user != null)
            {
                user.Parol = newPassword;
                user.Update();

                MessageBox.Show("Пароль успешно изменён! Теперь вы можете войти.", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.init.frame.Navigate(new Pages.Authorization());
            }
            else
            {
                MessageBox.Show("Ошибка обновления пароля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.DialogGetBackParol(_email));
        }
    }
}