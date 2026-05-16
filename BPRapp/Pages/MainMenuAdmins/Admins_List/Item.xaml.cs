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

        private void EditAdmin(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Add(user));
        }

        private void DeleteAdmin(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить администратора \"{user.FIO}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    user.Delete();
                    MessageBox.Show("Администратор удалён", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.init.frame.Navigate(new Admins_List());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"⚠️ {ex.Message}\n\n" +
                        $"Удалите эту запись из всех связанных таблиц, затем повторите попытку.",
                        "Удаление невозможно",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }
    }
}
