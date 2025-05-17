using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages
{
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        }

        private void Regin(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        }
    }
}
