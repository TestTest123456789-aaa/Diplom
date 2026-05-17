using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Departments
{
    public partial class Add : Page
    {
        Classes.Departments department;
        List<Classes.Users> AllTeachers;

        public Add(Classes.Departments department = null)
        {
            InitializeComponent();
            AllTeachers = Classes.Users.Select().Where(u => u.Role == "Преподаватель").ToList();

            var teacherList = new List<Classes.Users>
            {
                new Classes.Users(0, "Не назначен", "", "", "", "", "")
            };
            teacherList.AddRange(AllTeachers);
            headTeacherCB.ItemsSource = teacherList;
            headTeacherCB.DisplayMemberPath = "FIO";
            headTeacherCB.SelectedValuePath = "Id";

            if (department != null)
            {
                this.department = department;
                nameTB.Text = department.Name;

                if (department.HeadTeacherId.HasValue)
                {
                    headTeacherCB.SelectedValue = department.HeadTeacherId.Value;
                }
                else
                {
                    headTeacherCB.SelectedIndex = 0;
                }
                BthAdd.Content = "Изменить информацию";
            }
        }

        private void AddDepartment(object sender, RoutedEventArgs e)
        {
            // 🔹 Валидация: название не пустое
            if (string.IsNullOrWhiteSpace(nameTB.Text))
            {
                MessageBox.Show("Введите название отделения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                nameTB.Focus();
                return;
            }

            // 🔹 Валидация: проверка уникальности названия на уровне UI (доп. защита)
            var existingDepts = Classes.Departments.Select();
            bool nameExists = existingDepts.Any(d =>
                d.Name.ToLower() == nameTB.Text.Trim().ToLower() &&
                (department == null || d.Id != department.Id));

            if (nameExists)
            {
                MessageBox.Show($"Отделение с названием \"{nameTB.Text.Trim()}\" уже существует",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                nameTB.Focus();
                return;
            }

            int? headTeacherId = null;
            if (headTeacherCB.SelectedValue is int selectedId && selectedId > 0)
            {
                headTeacherId = selectedId;
            }

            try
            {
                if (this.department == null)
                {
                    Classes.Departments newDept = new Classes.Departments(0, nameTB.Text.Trim(), headTeacherId);
                    newDept.Add();
                    MessageBox.Show("✅ Отделение добавлено", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Classes.Departments newDept = new Classes.Departments(department.Id, nameTB.Text.Trim(), headTeacherId);
                    newDept.Update();
                    MessageBox.Show("✅ Информация изменена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                MainWindow.init.frame.Navigate(new Departments());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Departments());
        }
    }
}