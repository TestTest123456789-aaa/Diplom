using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Lessons
{
    public partial class Item : UserControl
    {
        Classes.Lessons lessons;
        public Item(Classes.Lessons lessons)
        {
            InitializeComponent();
            this.lessons = lessons;
            lessonTB.Text = lessons.Lesson;
        }

        private void EditLesson(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Add(lessons));
        }

        private void DeleteLesson(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить предмет?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    lessons.Delete();
                    MainWindow.init.frame.Navigate(new Lessons());
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