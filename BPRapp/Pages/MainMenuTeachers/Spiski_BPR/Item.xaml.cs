using BPRapp.Classes;
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

        // 🔹 Кэшируем данные чтобы не делать запросы каждый раз
        private static List<Users> _allTeachers;
        private static List<Classes.Lessons> _allLessons;
        private static List<Classes.Groups> _allGroups;
        private static List<Classes.Specialties> _allSpecialties;
        private static List<Rooms> _allRooms;

        static Item()
        {
            // Загружаем данные один раз при первом использовании
            _allTeachers = Users.Select().Where(u => u.Role == "Преподаватель").ToList();
            _allLessons = Classes.Lessons.Select();
            _allGroups = Classes.Groups.Select();
            _allSpecialties = Classes.Specialties.Select();
            _allRooms = Rooms.Select();
        }

        public Item(Classes.BPR_info bpr_info)
        {
            InitializeComponent();
            this.bpr = bpr_info;

            // Заполняем основные поля
            dateTB.Text = bpr.Date;
            timeTB.Text = bpr.Time;

            // Получаем количество студентов
            int actualCount = bpr_info.GetActualStudentCount();
            countTB.Text = actualCount.ToString();

            // 🔹 Используем кэшированные данные вместо запросов к БД
            respTB.Text = GetResponsibleTeacherName();
            lessonTB.Text = GetLessonName();
            groupTB.Text = GetGroupName();
            headTB.Text = GetGroupHeadTeacher();
            specTB.Text = GetGroupSpecialty();

            // Заполняем информацию о кабинете
            if (bpr_info.RoomId.HasValue)
            {
                var room = _allRooms.FirstOrDefault(r => r.Id == bpr_info.RoomId.Value);
                if (room != null)
                {
                    roomTB.Text = $"{room.Name} ({room.Capacity} мест)";

                    // Проверяем переполнение
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

        // 🔹 Методы для получения данных из кэша
        private string GetResponsibleTeacherName()
        {
            var teacher = _allTeachers.FirstOrDefault(t => t.Id == bpr.Responsible_user);
            return teacher?.FIO ?? "Не назначен";
        }

        private string GetLessonName()
        {
            var lesson = _allLessons.FirstOrDefault(l => l.Id == bpr.Lesson);
            return lesson?.Lesson ?? "Не указан";
        }

        private string GetGroupName()
        {
            if (!bpr.GroupId.HasValue) return "Не указана";
            var group = _allGroups.FirstOrDefault(g => g.Id == bpr.GroupId.Value);
            return group?.Name ?? "Не указана";
        }

        private string GetGroupHeadTeacher()
        {
            if (!bpr.GroupId.HasValue) return "—";
            var group = _allGroups.FirstOrDefault(g => g.Id == bpr.GroupId.Value);
            if (group?.HeadTeacherId == null) return "—";

            var teacher = _allTeachers.FirstOrDefault(t => t.Id == group.HeadTeacherId.Value);
            return teacher?.FIO ?? "—";
        }

        private string GetGroupSpecialty()
        {
            if (!bpr.GroupId.HasValue) return "Не указана";
            var group = _allGroups.FirstOrDefault(g => g.Id == bpr.GroupId.Value);
            if (group?.SpecialtyId == null) return "Не указана";

            var specialty = _allSpecialties.FirstOrDefault(s => s.Id == group.SpecialtyId.Value);
            return specialty != null ? $"{specialty.Code} \"{specialty.Name}\"" : "Не указана";
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