using BPRapp.Classes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Kabinets
{
    public partial class Item : UserControl
    {
        Rooms room;
        public Item(Rooms room)
        {
            InitializeComponent();
            this.room = room;
            nameTB.Text = room.Name;
            capacityTB.Text = room.Capacity.ToString();
        }
        private void EditKabinet(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Add(room));
        private void DeleteKabinet(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить кабинет?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    room.Delete();
                    MessageBox.Show("Кабинет успешно удалён", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.init.frame.Navigate(new Kabinets());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
        $"⚠️ {ex.Message}\n\n" +
        $"Удалите эту запись из всех связанных таблиц, затем повторите попытку.",
        "Удаление невозможно",
        MessageBoxButton.OK,
        MessageBoxImage.Warning);
                }
            }
        }
    }
}