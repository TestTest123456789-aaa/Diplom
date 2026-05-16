using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuAdmins.Admins_List
{
    public partial class Admins_List : Page
    {
        private List<Classes.Users> AllUsers = Classes.Users.Select();

        public Admins_List()
        {
            InitializeComponent();
            LoadAdmins();
        }

        private void LoadAdmins(string searchText = "")
        {
            Admins_ListParent.Children.Clear();

            var filtered = AllUsers
                .Where(u => u.Role == "Администратор")
                .Where(u => string.IsNullOrWhiteSpace(searchText) ||
                           u.FIO.ToLower().Contains(searchText.ToLower()) ||
                           u.Login.ToLower().Contains(searchText.ToLower()) ||
                           u.Email.ToLower().Contains(searchText.ToLower()));

            foreach (var user in filtered.OrderBy(u => u.FIO))
            {
                Admins_ListParent.Children.Add(new Item(user));
            }
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadAdmins(searchTB.Text);
        }

        private void OpenAdmins_List(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Admins_List());
        private void OpenTeachers_List(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
        private void OpenAuthorization(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void AddAdmin(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Add());
        private void OpenExportImport(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.ExportImport.ExportImport());
    }
}