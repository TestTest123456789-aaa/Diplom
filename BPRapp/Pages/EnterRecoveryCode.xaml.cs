using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class EnterRecoveryCode : Page
    {
        private readonly string _email;

        public EnterRecoveryCode(string email)
        {
            InitializeComponent();
            _email = email;
        }

        private void CodeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void VerifyCode(object sender, RoutedEventArgs e)
        {
            string code = codeTB.Text.Trim();

            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                statusText.Text = "Введите 6-значный код";
                statusText.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            if (EmailService.ValidateCode(_email, code))
            {
                EmailService.MarkCodeAsUsed(_email, code);
                MainWindow.init.frame.Navigate(new ChangePassword(_email));
            }
            else
            {
                statusText.Text = "❌ Неверный или истёкший код";
                statusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void ResendCode(object sender, RoutedEventArgs e)
        {
            //    string code = EmailService.GenerateCode();
            //    EmailService.SaveRecoveryCode(_email, code);

            //    if (EmailService.SendRecoveryCode(_email, code))
            //    {
            //        statusText.Text = "✅ Новый код отправлен";
            //        statusText.Foreground = System.Windows.Media.Brushes.Green;
            //    }
            //    else
            //    {
            //        statusText.Text = "❌ Ошибка отправки";
            //        statusText.Foreground = System.Windows.Media.Brushes.Red;
            //    }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.GetBackParol());
        }
    }
}