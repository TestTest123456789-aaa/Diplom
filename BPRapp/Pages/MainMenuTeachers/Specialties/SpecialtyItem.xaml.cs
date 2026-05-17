using System;
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
            MainWindow.init.frame.Navigate(new AddSpecialty(specialty, specialty.DepartmentId));
        }

        private void DeleteSpecialty(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить специальность?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    specialty.Delete();
                    int? deptId = specialty.DepartmentId;
                    MainWindow.init.frame.Navigate(new Specialties(deptId ?? 0));
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