using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BPRapp.Pages.MainMenuTeachers.Spiski_BPR
{
    public partial class Add : Page
    {
        List<Classes.Lessons> AllLessons = Classes.Lessons.Select();
        List<Users> AllTeachers = Users.Select().Where(u => u.Role == "Преподаватель").ToList();
        List<Classes.Groups> AllGroups = Classes.Groups.Select();
        List<Classes.Rooms> AllRooms = Classes.Rooms.Select();
        BPR_info bpr_info;

        private bool isLoading = true;
        private bool isChecking = false;

        public class GroupDisplayItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public Classes.Groups OriginalGroup { get; set; }

            public GroupDisplayItem(Classes.Groups group)
            {
                Id = group.Id;
                Name = group.Name;
                OriginalGroup = group;
                int studentCount = GetStudentCountForGroup(group.Id);
                DisplayName = $"{group.Name} ({studentCount} студента(-ов) )";
            }

            private int GetStudentCountForGroup(int groupId)
            {
                try
                {
                    var students = Classes.Student_Info.Select();
                    return students.Count(s => s.Group_name == groupId && s.Spisok_BPR_Id == null);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public Add(BPR_info bpr_info = null)
        {
            InitializeComponent();
            this.bpr_info = bpr_info;

            var groupDisplayItems = AllGroups.Select(g => new GroupDisplayItem(g)).ToList();
            groupCB.ItemsSource = groupDisplayItems;
            groupCB.DisplayMemberPath = "DisplayName";
            groupCB.SelectedValuePath = "Id";

            teacherCB.ItemsSource = AllTeachers;
            teacherCB.DisplayMemberPath = "FIO";
            teacherCB.SelectedValuePath = "Id";

            lessonCB.ItemsSource = AllLessons;
            lessonCB.DisplayMemberPath = "Lesson";
            lessonCB.SelectedValuePath = "Id";

            roomCB.ItemsSource = AllRooms;
            roomCB.DisplayMemberPath = "DisplayName";
            roomCB.SelectedValuePath = "Id";

            if (bpr_info != null)
            {
                DateTime? parsedDate = null;
                if (DateTime.TryParseExact(bpr_info.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    parsedDate = dt;
                }
                dateTB.SelectedDate = parsedDate;

                if (!string.IsNullOrEmpty(bpr_info.Time) && bpr_info.Time.Contains("-"))
                {
                    var parts = bpr_info.Time.Split('-');
                    startTimeTB.Text = parts[0].Trim();
                    endTimeTB.Text = parts[1].Trim();
                }

                roomCB.SelectedValue = bpr_info.RoomId;
                groupCB.SelectedValue = bpr_info.GroupId;
                teacherCB.SelectedValue = bpr_info.Responsible_user;
                lessonCB.SelectedValue = bpr_info.Lesson;
                BthAdd.Content = "Обновить запись";
            }

            this.Loaded += Add_Loaded;
        }

        private void Add_Loaded(object sender, RoutedEventArgs e)
        {
            isLoading = false;
            groupCB.SelectionChanged += GroupCB_SelectionChanged;
            roomCB.SelectionChanged += RoomCB_SelectionChanged;
        }

        private void GroupCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading || isChecking) return;

            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                CheckRoomCapacity();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void RoomCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoading || isChecking) return;

            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                CheckRoomCapacity();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private bool CheckRoomCapacity()
        {
            if (isChecking) return true;

            if (groupCB.SelectedItem == null || roomCB.SelectedItem == null)
                return true;

            var selectedGroupItem = groupCB.SelectedItem as GroupDisplayItem;
            var selectedRoom = roomCB.SelectedItem as Classes.Rooms;

            if (selectedGroupItem == null || selectedRoom == null)
                return true;

            int studentCount = GetStudentCountInGroup(selectedGroupItem.Id);

            int roomCapacity = selectedRoom.Capacity;

            if (studentCount > roomCapacity)
            {
                isChecking = true;

                MessageBox.Show(
                    $"⚠️ ВНИМАНИЕ!\n\n" +
                    $"В группе \"{selectedGroupItem.Name}\" {studentCount} студентов.\n" +
                    $"В кабинете \"{selectedRoom.Name}\" только {roomCapacity} мест.\n\n" +
                    $"Студентов больше, чем мест в кабинете!\n\n" +
                    $"Пожалуйста, выберите другой кабинет или измените группу.",
                    "Превышение вместимости кабинета",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                isChecking = false;
                return false;
            }

            return true;
        }

        private int GetStudentCountInGroup(int groupId)
        {
            var allStudents = Classes.Student_Info.Select();
            return allStudents.Count(s => s.Group_name == groupId && s.Spisok_BPR_Id == null);
        }

        private void AddBPR_info(object sender, RoutedEventArgs e)
        {
            if (!TimeSpan.TryParse(startTimeTB.Text.Trim(), out TimeSpan start) || !TimeSpan.TryParse(endTimeTB.Text.Trim(), out TimeSpan end))
            {
                MessageBox.Show("Введите корректное время в формате ЧЧ:ММ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (start >= end)
            {
                MessageBox.Show("Время начала должно быть меньше времени окончания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string formattedTime = $"{start:hh\\:mm} - {end:hh\\:mm}";
            if (!dateTB.SelectedDate.HasValue || groupCB.SelectedItem == null || teacherCB.SelectedItem == null || lessonCB.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? groupId = groupCB.SelectedValue as int?;
            int teacherId = (int)teacherCB.SelectedValue;
            int lessonId = (int)lessonCB.SelectedValue;
            int? roomId = roomCB.SelectedValue as int?;
            string dateStr = dateTB.SelectedDate.Value.ToString("dd.MM.yyyy");

            // ПРОВЕРКА ВМЕСТИМОСТИ КАБИНЕТА ПЕРЕД СОХРАНЕНИЕМ
            if (groupId.HasValue && roomId.HasValue)
            {
                var selectedRoom = AllRooms.FirstOrDefault(r => r.Id == roomId.Value);
                var selectedGroup = AllGroups.FirstOrDefault(g => g.Id == groupId.Value);

                if (selectedRoom != null && selectedGroup != null)
                {
                    int studentCount = GetStudentCountInGroup(groupId.Value);

                    if (studentCount > selectedRoom.Capacity)
                    {
                        var result = MessageBox.Show(
                            $"⚠️ ВНИМАНИЕ!\n\n" +
                            $"В группе \"{selectedGroup.Name}\" {studentCount} студентов.\n" +
                            $"В кабинете \"{selectedRoom.Name}\" только {selectedRoom.Capacity} мест.\n\n" +
                            $"Студентов больше, чем мест в кабинете!\n\n" +
                            $"Вы действительно хотите продолжить?",
                            "Превышение вместимости кабинета",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                            return;
                    }
                }
            }

            if (CheckScheduleConflicts(dateStr, formattedTime, teacherId, roomId, groupId, bpr_info?.Id ?? 0))
                return;

            if (bpr_info == null)
            {
                new BPR_info(0, dateStr, formattedTime, 0, teacherId, lessonId, groupId, roomId).Add();
                MessageBox.Show("ВПР создано");
            }
            else
            {
                int? oldGroupId = bpr_info.GroupId;
                bpr_info.Date = dateStr;
                bpr_info.Time = formattedTime;
                bpr_info.RoomId = roomId;
                bpr_info.GroupId = groupId;
                bpr_info.Responsible_user = teacherId;
                bpr_info.Lesson = lessonId;

                if (oldGroupId != groupId)
                {
                    var result = MessageBox.Show($"Группа изменена.\nОбновить список студентов?", "Синхронизация", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        bpr_info.UpdateWithStudentSync(oldGroupId);
                        MessageBox.Show($"✅ Список обновлён. Студентов: {bpr_info.GetActualStudentCount()}");
                    }
                    else
                    {
                        bpr_info.Update();
                        MessageBox.Show("Данные ВПР обновлены (список студентов не изменён)");
                    }
                }
                else
                {
                    bpr_info.Update();
                    MessageBox.Show("Данные обновлены");
                }
            }
            MainWindow.init.frame.Navigate(new Spiski_BPR());
        }

        private bool CheckScheduleConflicts(string dateStr, string timeRange, int teacherId, int? roomId, int? groupId, int excludeBprId)
        {
            var parts = timeRange.Split('-');
            if (!TimeSpan.TryParse(parts[0].Trim(), out TimeSpan start) || !TimeSpan.TryParse(parts[1].Trim(), out TimeSpan end))
                return false;

            DateTime dateBase = DateTime.ParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime newStart = dateBase.Add(start);
            DateTime newEnd = dateBase.Add(end);

            var allBPRs = BPR_info.Select();
            foreach (var bpr in allBPRs)
            {
                if (bpr.Id == excludeBprId) continue;
                if (bpr.Date != dateStr) continue;

                var bprParts = bpr.Time.Split('-');
                if (!TimeSpan.TryParse(bprParts[0].Trim(), out TimeSpan bprStart) || !TimeSpan.TryParse(bprParts[1].Trim(), out TimeSpan bprEnd))
                    continue;

                DateTime bprStartDT = dateBase.Add(bprStart);
                DateTime bprEndDT = dateBase.Add(bprEnd);
                if ((newStart < bprEndDT) && (newEnd > bprStartDT))
                {
                    if (bpr.Responsible_user == teacherId)
                    {
                        MessageBox.Show($"⛔ Конфликт: Преподаватель уже занят {bpr.Time}", "Ошибка расписания", MessageBoxButton.OK, MessageBoxImage.Error);
                        return true;
                    }
                    if (roomId.HasValue && bpr.RoomId == roomId.Value)
                    {
                        MessageBox.Show($"⛔ Конфликт: Кабинет уже занят {bpr.Time}", "Ошибка расписания", MessageBoxButton.OK, MessageBoxImage.Error);
                        return true;
                    }
                    if (groupId.HasValue && bpr.GroupId == groupId.Value)
                    {
                        MessageBox.Show($"⛔ Конфликт: Группа уже пишет ВПР {bpr.Time}", "Ошибка расписания", MessageBoxButton.OK, MessageBoxImage.Error);
                        return true;
                    }
                }
            }
            return false;
        }

        private string GetGroupNameById(int? id)
        {
            if (!id.HasValue) return "не указана";
            var group = Classes.Groups.Select().FirstOrDefault(g => g.Id == id.Value);
            return group?.Name ?? "неизвестно";
        }

        private void GoBack(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Spiski_BPR());
    }
}