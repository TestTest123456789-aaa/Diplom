using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuAdmins.Teachers_List
{
    public partial class Item : UserControl
    {
        Classes.Users user;
        public Item(Classes.Users user)
        {
            InitializeComponent();
            this.user = user;
            FIOTB.Text = user.FIO;
            loginTB.Text = user.Login;
            parolTB.Text = user.Parol;
            emailTB.Text = user.Email;
            phone_numberTB.Text = user.Phone_Number;
        }

        private void EditTeachers(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Add(user));
        }

        private void DeleteTeachers(object sender, RoutedEventArgs e)
        {
            user.Delete();
            Classes.Users.Select();
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
        }
    }
}
