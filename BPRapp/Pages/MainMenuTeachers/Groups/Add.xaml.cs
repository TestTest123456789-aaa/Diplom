using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Groups
{
    public partial class Add : Page
    {
        Classes.Groups group;
        private List<Classes.Specialties> _allSpecialties = new List<Classes.Specialties>();

        public Add(Classes.Groups group = null)
        {
            InitializeComponent();
            this.group = group;

            var teachers = Users.Select().Where(u => u.Role == "Преподаватель").ToList();
            headCB.ItemsSource = new[] { new Users(0, "Не назначен", "", "", "", "", "") }.Concat(teachers);

            var depts = Classes.Departments.Select();
            deptCB.ItemsSource = new[] { new Classes.Departments(0, "Не выбрано") }.Concat(depts);

            _allSpecialties = Classes.Specialties.Select();
            UpdateSpecialtiesList();

            if (group != null)
            {
                nameTB.Text = group.Name;
                headCB.SelectedValue = group.HeadTeacherId ?? 0;
                deptCB.SelectedValue = group.DepartmentId ?? 0;
                specCB.SelectedValue = group.SpecialtyId ?? 0;

                BthAdd.Content = "Обновить информацию";
            }
            else
            {
                headCB.SelectedIndex = 0;
                deptCB.SelectedIndex = 0;
            }
        }

        private void Dept_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSpecialtiesList();
        }

        private void UpdateSpecialtiesList()
        {
            int? deptId = deptCB.SelectedValue as int?;
            var filtered = new List<Classes.Specialties> { new Classes.Specialties(0, "", "Не указана") };

            if (deptId.HasValue && deptId.Value > 0)
            {
                filtered.AddRange(_allSpecialties.Where(s => s.DepartmentId == deptId.Value));
            }
            else
            {
                filtered.AddRange(_allSpecialties);
            }

            specCB.ItemsSource = filtered;

            if (specCB.SelectedValue is int selectedSpecId && selectedSpecId > 0)
            {
                bool isValid = filtered.Any(s => s.Id == selectedSpecId);
                if (!isValid) specCB.SelectedValue = 0;
            }
        }

        private void SaveGroup(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTB.Text))
            {
                MessageBox.Show("Введите название группы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int? head = headCB.SelectedValue as int?;
                int? dept = deptCB.SelectedValue as int?;
                int? spec = specCB.SelectedValue as int?;

                if (group == null)
                {
                    new Classes.Groups(0, nameTB.Text.Trim(), head, dept, spec).Add();
                    MessageBox.Show("Группа добавлена");
                }
                else
                {
                    new Classes.Groups(group.Id, nameTB.Text.Trim(), head, dept, spec).Update();
                    MessageBox.Show("Данные обновлены");
                }
                MainWindow.init.frame.Navigate(new Groups());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Groups());
    }
}