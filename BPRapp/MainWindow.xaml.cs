using System.Windows;

namespace BPRapp
{
    public partial class MainWindow : Window
    {
        public static MainWindow init;
        public MainWindow()
        {
            InitializeComponent();
            init = this;
            frame.Navigate(new Pages.Authorization());
        }
    }
}
