using Mysqlx.Notice;
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

namespace BPRapp.Pages
{
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void Authorize(object sender, RoutedEventArgs e)
        {

        }

        private void GetBackParol(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.GetBackParol());
        }

        private void OpenRegistration(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.Registration());
        }
    }
}
