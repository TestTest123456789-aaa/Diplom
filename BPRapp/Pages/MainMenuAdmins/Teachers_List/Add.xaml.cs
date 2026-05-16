using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuAdmins.Teachers_List
{
    public partial class Add : Page
    {
        Classes.Users users;
        public Add(Classes.Users users = null)
        {
            InitializeComponent();
            if (users != null)
            {
                this.users = users;
                FIOTB.Text = users.FIO;
                loginTB.Text = users.Login;
                parolTB.Text = users.Parol;
                users.Role = "Преподаватель";
                emailTB.Text = users.Email;
                phone_numberTB.Text = users.Phone_Number;
                BthAdd.Content = $"Изменить запись";
            }
        }

        private void AddTeacher(object sender, RoutedEventArgs e)
        {
            if (this.users == null)
            {
                Classes.Users newUsers = new Classes.Users(
                    0,
                    FIOTB.Text,
                    loginTB.Text,
                    parolTB.Text,
                    "Преподаватель",
                    emailTB.Text,
                    phone_numberTB.Text
                );
                newUsers.Add();
                MessageBox.Show("Преподаватель добавлен");
            }
            else
            {
                Classes.Users newUsers = new Classes.Users(
                    users.Id,
                    FIOTB.Text,
                    loginTB.Text,
                    parolTB.Text,
                    "Преподаватель",
                    emailTB.Text,
                    phone_numberTB.Text
                );
                newUsers.Update();
                MessageBox.Show("Информация изменена");
            }
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
        }
    }
}
