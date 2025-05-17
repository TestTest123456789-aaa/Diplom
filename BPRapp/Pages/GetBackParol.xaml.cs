using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages
{
    public partial class GetBackParol : Page
    {
        public GetBackParol()
        {
            InitializeComponent();
        }

        private void SendEmail(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.DialogGetBackParol());
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        }
    }
}
