using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuAdmins
{
    public partial class MainMenuAdmin : Page
    {
        public MainMenuAdmin()
        {
            InitializeComponent();
        }
        private void OpenAdmins_List(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Admins_List());
        private void OpenTeachers_List(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
        private void OpenAuthorization(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenExportImport(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.ExportImport.ExportImport());
    }
}
