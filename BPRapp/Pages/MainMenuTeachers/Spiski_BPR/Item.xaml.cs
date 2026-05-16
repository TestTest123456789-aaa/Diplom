using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BPRapp.Pages.MainMenuTeachers.Spiski_BPR
{
    public partial class Item : UserControl
    {
        private Classes.BPR_info bpr;

        public Item(Classes.BPR_info bpr_info)
        {
            InitializeComponent();
            this.bpr = bpr_info;

            dateTB.Text = bpr.Date;
            timeTB.Text = bpr.Time;

            int actualCount = bpr_info.GetActualStudentCount();
            countTB.Text = actualCount.ToString();

            respTB.Text = bpr.GetResponsibleTeacherName();
            lessonTB.Text = bpr.GetLessonName();
            groupTB.Text = bpr.GetGroupName();
            headTB.Text = bpr.GetGroupHeadTeacher();
            specTB.Text = bpr.GetGroupSpecialty();

            if (bpr_info.RoomId.HasValue)
            {
                var room = Classes.Rooms.Select().FirstOrDefault(r => r.Id == bpr_info.RoomId.Value);
                if (room != null)
                {
                    roomTB.Text = $"{room.Name} ({room.Capacity} мест)";

                    if (actualCount > room.Capacity)
                    {
                        countTB.Foreground = Brushes.Red;
                        roomTB.Foreground = Brushes.Red;
                        roomTB.ToolTip = $"⚠️ ВНИМАНИЕ!\nВместимость кабинета: {room.Capacity}\nСтудентов: {actualCount}\nМест не хватает!";
                    }
                    else
                    {
                        countTB.Foreground = Brushes.Black;
                        roomTB.Foreground = Brushes.Black;
                    }
                }
            }
        }

        private void EditSpisok_BPR(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Add(bpr));

        private void DeleteSpisok_BPR(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить запись ВПР?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                bpr.Delete();
                MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
            }
        }

        private void OpenSpisok(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_Students.Spiski_Students(bpr.Id));
    }
}