using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Departments
{
    public partial class Item : UserControl
    {
        Classes.Departments department;

        public Item(Classes.Departments department)
        {
            InitializeComponent();
            this.department = department;
            nameTB.Text = department.Name;
            headTB.Text = department.GetHeadTeacherName();
        }

        private void EditDepartment(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Add(department));
        }

        private void DeleteDepartment(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить отделение?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    department.Delete();
                    MainWindow.init.frame.Navigate(new Departments());
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

        private void OpenSpecialties(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Specialties.Specialties(department.Id));
        }
    }
}