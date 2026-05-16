using BPRapp.Classes;
using BPRapp.Pages.MainMenuTeachers.Spiski_Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPRapp.Pages.MainMenuTeachers.StudentsList
{
    public partial class StudentsList : Page
    {
        private List<Student_Info> AllStudents = new List<Student_Info>();
        private List<Classes.Groups> AllGroups = new List<Classes.Groups>();

        public StudentsList()
        {
            InitializeComponent();
            LoadData();
            SetupFilters();
            LoadStudents();
        }

        private void LoadData()
        {
            AllStudents = Student_Info.Select();
            AllGroups = Classes.Groups.Select();
        }

        private void SetupFilters()
        {
            filterGroupCB.Items.Add(new Classes.Groups(0, "Все группы"));
            foreach (var g in AllGroups)
                filterGroupCB.Items.Add(g);
            filterGroupCB.SelectedIndex = 0;
        }

        private void LoadStudents()
        {
            StudentsParent.Children.Clear();

            var filtered = AllStudents.AsEnumerable();

            if (filterGroupCB.SelectedValue is int groupId && groupId > 0)
            {
                filtered = filtered.Where(s => s.Group_name == groupId);
            }

            foreach (var student in filtered.OrderBy(s => s.FIO))
            {
                // ← Используем НОВЫЙ контрол StudentItem вместо Spiski_Students.Item
                StudentsParent.Children.Add(new StudentItem(student));
            }
        }

        private void ApplyFilters(object sender, SelectionChangedEventArgs e)
        {
            LoadStudents();
        }

        private void ResetFilters(object sender, RoutedEventArgs e)
        {
            filterGroupCB.SelectedIndex = 0;
            LoadStudents();
        }

        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
        private void OpenRaspisanie_BPR(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Raspisanie_BPR.Raspisanie_BPR());
        private void OpenClosed_BRP(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Closed_BRP.Closed_BRP());
        private void OpenGroups(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Groups.Groups());
        private void OpenMainMenu(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.MainMenuTeachers());
    }
}