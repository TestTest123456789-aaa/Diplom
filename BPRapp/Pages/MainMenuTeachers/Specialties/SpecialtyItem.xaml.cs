using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Specialties
{
    public partial class SpecialtyItem : UserControl
    {
        Classes.Specialties specialty;

        public SpecialtyItem(Classes.Specialties specialty)
        {
            InitializeComponent();
            this.specialty = specialty;
            codeTB.Text = specialty.Code;
            nameTB.Text = specialty.Name;
        }

        private void EditSpecialty(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new AddSpecialty(specialty));
        }

        private void DeleteSpecialty(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить специальность?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                specialty.Delete();
                int? deptId = specialty.DepartmentId;
                MainWindow.init.frame.Navigate(new Specialties(deptId ?? 0));
            }
        }
    }
}