using BPRapp.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BPRapp.Pages.MainMenuTeachers.Kabinets
{
    public partial class Add : Page
    {
        Rooms room;
        public Add(Rooms room = null)
        {
            InitializeComponent();
            this.room = room;
            if (room != null)
            {
                nameTB.Text = room.Name;
                capacityTB.Text = room.Capacity.ToString();
                BthAdd.Content = "Обновить информацию";
            }
        }
        private void SaveKabinet(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTB.Text))
            {
                MessageBox.Show("Введите название кабинета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(capacityTB.Text, out int cap) || cap <= 0)
            {
                MessageBox.Show("Укажите корректное количество мест", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (room == null)
                {
                    new Rooms(0, nameTB.Text.Trim(), cap).Add();
                    MessageBox.Show("Кабинет добавлен");
                }
                else
                {
                    new Rooms(room.Id, nameTB.Text.Trim(), cap).Update();
                    MessageBox.Show("Данные обновлены");
                }
                MainWindow.init.frame.Navigate(new Kabinets());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GoBack(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Kabinets());

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}