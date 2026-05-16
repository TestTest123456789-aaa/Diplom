using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuAdmins.Teachers_List
{
    public partial class Teachers_List : Page
    {
        private List<Classes.Users> AllUsers = Classes.Users.Select();

        public Teachers_List()
        {
            InitializeComponent();
            LoadTeachers();
        }

        private void LoadTeachers(string searchText = "")
        {
            Teachers_ListParent.Children.Clear();

            var filtered = AllUsers
                .Where(u => u.Role == "Преподаватель")
                .Where(u => string.IsNullOrWhiteSpace(searchText) ||
                           u.FIO.ToLower().Contains(searchText.ToLower()) ||
                           u.Login.ToLower().Contains(searchText.ToLower()) ||
                           u.Email.ToLower().Contains(searchText.ToLower()));

            foreach (var user in filtered.OrderBy(u => u.FIO))
            {
                Teachers_ListParent.Children.Add(new Item(user));
            }
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadTeachers(searchTB.Text);
        }

        private void OpenAdmins_List(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Admins_List());
        private void OpenTeachers_List(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Teachers_List());
        private void OpenAuthorization(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenExportImport(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.ExportImport.ExportImport());
        private void AddTeacher(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Add());
    }
}
