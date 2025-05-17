using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages
{
    public partial class DialogGetBackParol : Page
    {
        public DialogGetBackParol()
        {
            InitializeComponent();
        }

        private void SendKode(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.NewParol());
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.GetBackParol());
        }
    }
}
