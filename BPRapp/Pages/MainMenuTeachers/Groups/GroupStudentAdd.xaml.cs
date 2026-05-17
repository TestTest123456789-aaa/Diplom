using BPRapp.Classes;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Groups
{
    public partial class GroupStudentAdd : Page
    {
        private int _groupId;
        private Student_Info _student;
        public GroupStudentAdd(int groupId) : this(groupId, null) { }
        public GroupStudentAdd(int groupId, Student_Info student)
        {
            InitializeComponent();
            _groupId = groupId;
            _student = student;

            if (student != null)
            {
                fioTB.Text = student.FIO;
                polCB.Text = student.Pol;
                formaCB.Text = student.Forma_obychenya;
                saveBtn.Content = "Обновить";
            }
        }

        private void SaveStudent(object sender, RoutedEventArgs e)
        {
            string pol = (polCB.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string forma = (formaCB.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(fioTB.Text))
            {
                MessageBox.Show("Введите ФИО", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_student == null)
            {
                var newStudent = new Student_Info(
                    0,
                    "",
                    pol,
                    "",
                    fioTB.Text,
                    "",
                    "",
                    "",
                    "",
                    forma,
                    _groupId,
                    null
                );

                if (newStudent.Add(out string errorMessage))
                {
                    MessageBox.Show("Студент добавлен в группу", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(errorMessage, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                _student.FIO = fioTB.Text;
                _student.Pol = pol;
                _student.Forma_obychenya = forma;
                _student.Update();
                MessageBox.Show("Данные обновлены");
            }

            MainWindow.init.frame.Navigate(new GroupStudents(_groupId));
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new GroupStudents(_groupId));
        }
    }
}