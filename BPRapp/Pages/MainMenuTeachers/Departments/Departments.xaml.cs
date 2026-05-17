using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Departments
{
    public partial class Departments : Page
    {
        private List<Classes.Departments> AllDepartments = new List<Classes.Departments>();

        public Departments()
        {
            InitializeComponent();
            Loaded += Departments_Loaded;
        }

        private void Departments_Loaded(object sender, RoutedEventArgs e)
        {
            if (FioLbl != null)
            {
                if (Classes.CurrentUser.IsAuthenticated && !string.IsNullOrEmpty(Classes.CurrentUser.FIO))
                {
                    FioLbl.Text = Classes.CurrentUser.FIO;

                    string contactInfo = "";
                    if (!string.IsNullOrEmpty(Classes.CurrentUser.Email)) contactInfo += Classes.CurrentUser.Email;
                    if (!string.IsNullOrEmpty(Classes.CurrentUser.Phone))
                    {
                        if (!string.IsNullOrEmpty(contactInfo)) contactInfo += " | ";
                        contactInfo += Classes.CurrentUser.Phone;
                    }

                    ContactLbl.Text = string.IsNullOrEmpty(contactInfo) ? "Контакты не указаны" : contactInfo;
                }
                else
                {
                    FioLbl.Text = "Пользователь не авторизован";
                    ContactLbl.Text = "";
                }
            }
            LoadDepartments();
        }

        private void LoadDepartments(string searchText = "")
        {
            AllDepartments = Classes.Departments.Select();

            var filtered = string.IsNullOrWhiteSpace(searchText)
                ? AllDepartments
                : AllDepartments.Where(d => d.Name.ToLower().Contains(searchText.ToLower()));

            DepartmentsParent.Children.Clear();
            foreach (var dept in filtered.OrderBy(d => d.Name))
            {
                DepartmentsParent.Children.Add(new Item(dept));
            }
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadDepartments(searchTB.Text);
        }

        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
        private void OpenRaspisanie_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Raspisanie_BPR.Raspisanie_BPR());
        private void OpenClosed_BRP(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Closed_BRP.Closed_BRP());
        private void OpenGroups(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Groups.Groups());
        private void OpenDepartments(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
        private void OpenAuthorization(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenExport(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Export.Export());
        private void OpenLessons(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
        private void OpenKabinets(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Kabinets.Kabinets());
        private void AddDepartment(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Add());

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