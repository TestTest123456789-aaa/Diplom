using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuAdmins.Admins_List
{
    public partial class Add : Page
    {
        private Classes.Users _users;

        public Add(Classes.Users users = null)
        {
            InitializeComponent();
            if (users != null)
            {
                _users = users;
                FIOTB.Text = users.FIO;
                loginTB.Text = users.Login;
                parolTB.Text = users.Parol;
                users.Role = "Администратор";
                emailTB.Text = users.Email;
                phone_numberTB.Text = users.Phone_Number;
                BthAdd.Content = $"✏️ Изменить";
            }
        }

        private void AddAdmin(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FIOTB.Text) || string.IsNullOrWhiteSpace(loginTB.Text) || string.IsNullOrWhiteSpace(parolTB.Text))
            {
                MessageBox.Show("❗ Заполните все обязательные поля (*).", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_users == null)
                {
                    var newUsers = new Classes.Users(
                        0, FIOTB.Text, loginTB.Text, parolTB.Text, "Администратор", emailTB.Text, phone_numberTB.Text);

                    if (!newUsers.ValidateNoDuplicates(out string errorMsg))
                    {
                        MessageBox.Show(errorMsg, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    newUsers.Add();
                    MessageBox.Show("✅ Администратор успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var updatedUsers = new Classes.Users(
                        _users.Id, FIOTB.Text, loginTB.Text, parolTB.Text, "Администратор", emailTB.Text, phone_numberTB.Text);

                    if (!updatedUsers.ValidateNoDuplicates(out string errorMsg))
                    {
                        MessageBox.Show(errorMsg, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    updatedUsers.Update();
                    MessageBox.Show("✅ Информация успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Admins_List());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Admins_List());
        }
    }
}