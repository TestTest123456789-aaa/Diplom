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
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (this.specialty == null)
            {
                Classes.Specialties newSpec = new Classes.Specialties(
                    0, codeTB.Text, nameTB.Text, departmentId);
                newSpec.Add();
                MessageBox.Show("Специальность добавлена");
            }
            else
            {
                Classes.Specialties updatedSpec = new Classes.Specialties(
                    specialty.Id, codeTB.Text, nameTB.Text, specialty.DepartmentId);
                updatedSpec.Update();
                MessageBox.Show("Информация изменена");
            }

            if (departmentId.HasValue)
            {
                MainWindow.init.frame.Navigate(new Specialties(departmentId.Value));
            }
            else
            {
                MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
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