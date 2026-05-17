using BPRapp.Classes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages
{
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void Authorize(object sender, RoutedEventArgs e)
        {
            string login = loginTB.Text.Trim();
            string password = parolTB.Text;

            // Администратор по умолчанию
            if (login == "sa" && password == "sa")
            {
                CurrentUser.SetUser(0, "Администратор", "Администратор", "", "");
                MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.MainMenuAdmin());
                return;
            }

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("⚠️ Введите логин и пароль", "Ошибка авторизации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = Users.Select().FirstOrDefault(u => u.Login == login && u.Parol == password);

            if (user == null)
            {
                MessageBox.Show("❌ Неверный логин или пароль", "Ошибка авторизации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentUser.SetUser(user.Id, user.Role, user.FIO, user.Email ?? "", user.Phone_Number ?? "");

            switch (user.Role)
            {
                case "Администратор":
                    MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.MainMenuAdmin());
                    break;
                case "Преподаватель":
                    MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.MainMenuTeachers());
                    break;
                default:
                    MessageBox.Show("❌ Неизвестная роль пользователя", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void GoToRecovery(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.GetBackParol());
        }
    }
}