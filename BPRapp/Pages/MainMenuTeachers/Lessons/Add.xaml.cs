using System;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Lessons
{
    public partial class Add : Page
    {
        Classes.Lessons lessons;
        public Add(Classes.Lessons lessons = null)
        {
            InitializeComponent();
            if (lessons != null)
            {
                this.lessons = lessons;
                lessonTB.Text = lessons.Lesson;
                BthAdd.Content = $"Изменить информацию";
            }
        }

        private void AddLesson(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lessonTB.Text))
            {
                MessageBox.Show("Введите название предмета", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (this.lessons == null)
                {
                    Classes.Lessons newLessons = new Classes.Lessons(0, lessonTB.Text.Trim());
                    newLessons.Add();
                    MessageBox.Show("Предмет добавлен");
                }
                else
                {
                    Classes.Lessons newLessons = new Classes.Lessons(lessons.Id, lessonTB.Text.Trim());
                    newLessons.Update();
                    MessageBox.Show("Информация изменена");
                }
                MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
        }
    }
}
