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
                filtered.AddRange(_allSpecialties.Where(s => s.DepartmentId == deptId.Value));
            else
                filtered.AddRange(_allSpecialties);
            specCB.ItemsSource = filtered;
            if (specCB.SelectedValue is int selectedSpecId && selectedSpecId > 0)
            {
                bool isValid = filtered.Any(s => s.Id == selectedSpecId);
                if (!isValid) specCB.SelectedValue = 0;
            }
        }

        private void SaveGroup(object sender, RoutedEventArgs e)
        {
            // 🔹 ВАЛИДАЦИЯ: Все поля обязательны
            if (string.IsNullOrWhiteSpace(nameTB.Text))
            {
                MessageBox.Show("❗ Введите название группы", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                nameTB.Focus();
                return;
            }
            if (headCB.SelectedValue == null || (headCB.SelectedValue is int h && h == 0))
            {
                MessageBox.Show("❗ Выберите руководителя группы", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                headCB.Focus();
                return;
            }
            if (deptCB.SelectedValue == null || (deptCB.SelectedValue is int d && d == 0))
            {
                MessageBox.Show("❗ Выберите отделение", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                deptCB.Focus();
                return;
            }
            if (specCB.SelectedValue == null || (specCB.SelectedValue is int s && s == 0))
            {
                MessageBox.Show("❗ Выберите специальность", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                specCB.Focus();
                return;
            }

            try
            {
                int? head = headCB.SelectedValue is int hVal && hVal > 0 ? hVal : (int?)null;
                int? dept = deptCB.SelectedValue is int dVal && dVal > 0 ? dVal : (int?)null;
                int? spec = specCB.SelectedValue is int sVal && sVal > 0 ? sVal : (int?)null;

                if (group == null)
                {
                    new Classes.Groups(0, nameTB.Text.Trim(), head, dept, spec).Add();
                    MessageBox.Show("✅ Группа успешно добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    new Classes.Groups(group.Id, nameTB.Text.Trim(), head, dept, spec).Update();
                    MessageBox.Show("✅ Данные группы обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                MainWindow.init.frame.Navigate(new Groups());
            }
            catch (Exception ex)
            {
                // 🔹 Отображаем понятное сообщение на русском
                string errorMsg = ex.Message;
                if (errorMsg.Contains("already exists") || errorMsg.Contains("уже существует"))
                    errorMsg = $"⚠️ Группа с названием \"{nameTB.Text.Trim()}\" уже существует. Пожалуйста, выберите другое название.";
                else if (errorMsg.Contains("cannot be null") || errorMsg.Contains("не может быть пустым"))
                    errorMsg = "⚠️ Все поля формы обязательны для заполнения.";

                MessageBox.Show(errorMsg, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Groups());
    }
}