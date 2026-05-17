using System;
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
            var result = MessageBox.Show(
                $"⚠️ Вы действительно хотите удалить преподавателя \"{user.FIO}\"?\n\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    user.Delete();
                    MessageBox.Show($"✅ Преподаватель \"{user.FIO}\" успешно удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Невозможно удалить", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}