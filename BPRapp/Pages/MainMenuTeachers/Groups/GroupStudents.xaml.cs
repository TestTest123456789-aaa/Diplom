using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BPRapp.Pages.MainMenuTeachers.Groups
{
    public partial class GroupStudents : Page
    {
        private int _groupId;
        public GroupStudents(int groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            var group = Classes.Groups.Select().FirstOrDefault(g => g.Id == groupId);
            TitleLbl.Content = $"Студенты группы {group?.Name ?? ""}";
            LoadStudents();
        }

        private void LoadStudents()
        {
            ListParent.Children.Clear();
            var students = Classes.Student_Info.Select().Where(s => s.Group_name == _groupId && s.Spisok_BPR_Id == null).OrderBy(s => s.FIO).ToList();

            CountLbl.Content = $"Всего студентов: {students.Count}";

            int index = 1;
            foreach (var s in students)
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

                var num = new TextBlock
                {
                    Text = $"{index}.",
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                Grid.SetColumn(num, 0);
                grid.Children.Add(num);

                var fio = new TextBlock
                {
                    Text = s.FIO,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                Grid.SetColumn(fio, 1);
                grid.Children.Add(fio);

                var btnEdit = new Button
                {
                    Content = "✏️",
                    Margin = new Thickness(2),
                    Background = Brushes.Green,
                    Foreground = Brushes.White,
                    ToolTip = "Изменить (только ФИО/Пол/Форма)"
                };
                btnEdit.Click += (se, ev) => MainWindow.init.frame.Navigate(new GroupStudentAdd(_groupId, s));
                Grid.SetColumn(btnEdit, 2);
                grid.Children.Add(btnEdit);

                var btnDel = new Button
                {
                    Content = "🗑️",
                    Margin = new Thickness(2),
                    Background = Brushes.Red,
                    Foreground = Brushes.White,
                    ToolTip = "Удалить студента"
                };
                btnDel.Click += (se, ev) => {
                    if (MessageBox.Show($"Удалить студента {s.FIO}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        s.Delete();
                        LoadStudents();
                    }
                };
                Grid.SetColumn(btnDel, 3);
                grid.Children.Add(btnDel);

                border.Child = grid;
                ListParent.Children.Add(border);
                index++;
            }
        }

        private void AddStudent(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new GroupStudentAdd(_groupId));
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Groups());
        }
    }
}