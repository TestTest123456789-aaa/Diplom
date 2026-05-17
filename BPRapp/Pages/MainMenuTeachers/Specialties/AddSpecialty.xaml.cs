using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Specialties
{
    public partial class AddSpecialty : Page
    {
        Classes.Specialties specialty;
        private int? departmentId;

        public AddSpecialty(Classes.Specialties specialty = null, int? deptId = null)
        {
            InitializeComponent();
            this.specialty = specialty;
            this.departmentId = deptId;

            if (specialty != null)
            {
                codeTB.Text = specialty.Code;
                nameTB.Text = specialty.Name;
                BthAdd.Content = "Изменить";
            }
        }

        private void SaveSpecialty(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(codeTB.Text) || string.IsNullOrWhiteSpace(nameTB.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (this.specialty == null)
                {
                    Classes.Specialties newSpec = new Classes.Specialties(
                        0, codeTB.Text.Trim(), nameTB.Text.Trim(), departmentId);
                    newSpec.Add();
                    MessageBox.Show("✅ Специальность добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Classes.Specialties updatedSpec = new Classes.Specialties(
                        specialty.Id, codeTB.Text.Trim(), nameTB.Text.Trim(), specialty.DepartmentId);
                    updatedSpec.Update();
                    MessageBox.Show("✅ Информация изменена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // 🔹 Возврат на страницу, с которой пришли
                GoBack(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (departmentId.HasValue)
            {
                MainWindow.init.frame.Navigate(new Specialties(departmentId.Value));
            }
            else
            {
                MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
            }
        }
    }
}