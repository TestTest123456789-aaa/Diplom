using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BPRapp.Pages.MainMenuTeachers.Groups
{
    public partial class GroupStudents : Page
    {
        private int _groupId;
        private List<Classes.Student_Info> _allStudents = new List<Classes.Student_Info>();

        public GroupStudents(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            var group = Classes.Groups.Select().FirstOrDefault(g => g.Id == groupId);
            TitleLbl.Text = $"Студенты группы {group?.Name ?? ""}";
            LoadStudents();
        }

        private void LoadStudents(string searchText = "")
        {
            ListParent.Children.Clear();

            // Загружаем всех студентов группы (не привязанных к ВПР)
            _allStudents = Classes.Student_Info.Select()
                .Where(s => s.Group_name == _groupId && s.Spisok_BPR_Id == null)
                .OrderBy(s => s.FIO)
                .ToList();

            // Фильтрация по поиску
            var filtered = string.IsNullOrWhiteSpace(searchText)
                ? _allStudents
                : _allStudents.Where(s => s.FIO.ToLower().Contains(searchText.ToLower()));

            // ✅ Теперь CountLbl существует в XAML — ошибки не будет
            CountLbl.Text = $"Всего студентов: {filtered.Count()}";

            int index = 1;
            foreach (var s in filtered)
            {
                var border = new Border
                {
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    Background = Brushes.White,
                    BorderBrush = Brushes.CadetBlue,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

                var num = new TextBlock { Text = $"{index}.", FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 5, 0) };
                Grid.SetColumn(num, 0);
                grid.Children.Add(num);

                var fio = new TextBlock { Text = s.FIO, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 5, 0) };
                Grid.SetColumn(fio, 1);
                grid.Children.Add(fio);

                var btnEdit = new Button { Content = "✏️", Margin = new Thickness(2), Background = Brushes.Green, Foreground = Brushes.White, ToolTip = "Изменить" };
                btnEdit.Click += (se, ev) => MainWindow.init.frame.Navigate(new GroupStudentAdd(_groupId, s));
                Grid.SetColumn(btnEdit, 2);
                grid.Children.Add(btnEdit);

                var btnDel = new Button { Content = "🗑️", Margin = new Thickness(2), Background = Brushes.Red, Foreground = Brushes.White, ToolTip = "Удалить" };
                btnDel.Click += (se, ev) => {
                    if (MessageBox.Show($"Удалить студента {s.FIO}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        s.Delete();
                        LoadStudents(searchStudentTB.Text);
                    }
                };
                Grid.SetColumn(btnDel, 3);
                grid.Children.Add(btnDel);

                border.Child = grid;
                ListParent.Children.Add(border);
                index++;
            }
        }

        private void SearchStudent_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadStudents(searchStudentTB.Text);
        }

        private void AddStudent(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new GroupStudentAdd(_groupId));
        private void GoBack(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Groups());

        // 🔹 Навигация
        private void OpenAuthorization(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
        private void OpenRaspisanie_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Raspisanie_BPR.Raspisanie_BPR());
        private void OpenClosed_BRP(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Closed_BRP.Closed_BRP());
        private void OpenExport(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Export.Export());
        private void OpenLessons(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
        private void OpenKabinets(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Kabinets.Kabinets());
        private void OpenGroups(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Groups());
        private void OpenDepartments(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
        private void OpenNotificationSettings(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Classes.CurrentUser.Email))
            {
                System.Windows.MessageBox.Show("У вас не указан Email. Уведомления недоступны.", "Внимание", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }
            MainWindow.init.frame.Navigate(new Pages.NotificationSettings(Classes.CurrentUser.Email));
        }
    }
}