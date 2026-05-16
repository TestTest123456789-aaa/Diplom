using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BPRapp.Pages.MainMenuAdmins.Admins_List
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
                users.Role = "Администратор";
                emailTB.Text = users.Email;
                phone_numberTB.Text = users.Phone_Number;
                BthAdd.Content = $"Изменить запись";
            }
        }

        private void AddAdmin(object sender, RoutedEventArgs e)
        {
            if (this.users == null)
            {
                Classes.Users newUsers = new Classes.Users(
                    0,
                    FIOTB.Text,
                    loginTB.Text,
                    parolTB.Text,
                    "Администратор",
                    emailTB.Text,
                    phone_numberTB.Text
                );
                newUsers.Add();
                MessageBox.Show("Администратор добавлен");
            }
            else
            {
                Classes.Users newUsers = new Classes.Users(
                    users.Id,
                    FIOTB.Text,
                    loginTB.Text,
                    parolTB.Text,
                    "Администратор",
                    emailTB.Text,
                    phone_numberTB.Text
                );
                newUsers.Update();
                MessageBox.Show("Информация изменена");
            }
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Admins_List());
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Admins_List());
        }
    }
}
