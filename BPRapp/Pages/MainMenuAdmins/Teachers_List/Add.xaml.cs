using System;
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
                BthAdd.Content = "✏️ Изменить запись";
            }
        }

        private bool ValidateInput(out string error)
        {
            error = "";

            if (string.IsNullOrWhiteSpace(FIOTB.Text))
            {
                error = "❗ Введите ФИО преподавателя";
                return false;
            }
            if (string.IsNullOrWhiteSpace(loginTB.Text) || loginTB.Text.Length < 3)
            {
                error = "❗ Логин должен содержать минимум 3 символа";
                return false;
            }
            if (string.IsNullOrWhiteSpace(parolTB.Text) || parolTB.Text.Length < 6)
            {
                error = "❗ Пароль должен содержать минимум 6 символов";
                return false;
            }
            if (!string.IsNullOrWhiteSpace(emailTB.Text) && !emailTB.Text.Contains("@"))
            {
                error = "❗ Введите корректный Email";
                return false;
            }
            if (!string.IsNullOrWhiteSpace(phone_numberTB.Text) && phone_numberTB.Text.Replace(" ", "").Replace("-", "").Length < 10)
            {
                error = "❗ Введите корректный номер телефона";
                return false;
            }
            return true;
        }

        private void AddTeacher(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput(out string validationError))
                {
                    MessageBox.Show(validationError, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (this.users == null)
                {
                    var newUsers = new Classes.Users(
                        0,
                        FIOTB.Text.Trim(),
                        loginTB.Text.Trim(),
                        parolTB.Text,
                        "Преподаватель",
                        string.IsNullOrWhiteSpace(emailTB.Text) ? null : emailTB.Text.Trim(),
                        string.IsNullOrWhiteSpace(phone_numberTB.Text) ? null : phone_numberTB.Text.Trim()
                    );
                    newUsers.Add();
                    MessageBox.Show("✅ Преподаватель успешно добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var updatedUsers = new Classes.Users(
                        users.Id,
                        FIOTB.Text.Trim(),
                        loginTB.Text.Trim(),
                        parolTB.Text,
                        "Преподаватель",
                        string.IsNullOrWhiteSpace(emailTB.Text) ? null : emailTB.Text.Trim(),
                        string.IsNullOrWhiteSpace(phone_numberTB.Text) ? null : phone_numberTB.Text.Trim()
                    );
                    updatedUsers.Update();
                    MessageBox.Show("✅ Информация успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
        }
    }
}