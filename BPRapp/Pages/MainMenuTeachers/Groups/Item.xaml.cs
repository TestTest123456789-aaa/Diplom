using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Groups
{
    public partial class Item : UserControl
    {
        Classes.Groups group;
        public Item(Classes.Groups group)
        {
            InitializeComponent();
            this.group = group;
            nameTB.Text = group.Name;
            headTB.Text = group.GetHeadTeacherName();
            deptTB.Text = group.GetDepartmentName();
            specTB.Text = group.GetSpecialtyName();
        }

        private void EditGroup(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Add(group));

        private void DeleteGroup(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить группу?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    group.Delete();
                    MessageBox.Show("Группа успешно удалена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.init.frame.Navigate(new Groups());
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

        private void OpenStudents(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new GroupStudents(group.Id));
        }
    }
}