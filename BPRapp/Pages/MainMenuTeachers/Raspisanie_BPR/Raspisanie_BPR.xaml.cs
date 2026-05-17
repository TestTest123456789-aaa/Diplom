using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Raspisanie_BPR
{
    public partial class Raspisanie_BPR : Page
    {
        private List<BPR_info> AllBPRs = new List<BPR_info>();
        private bool _showMyOnly = false;

        public Raspisanie_BPR()
        {
            InitializeComponent();
            Loaded += Raspisanie_BPR_Loaded;
        }

        private void Raspisanie_BPR_Loaded(object sender, RoutedEventArgs e)
        {
            if (FioLbl != null)
            {
                if (Classes.CurrentUser.IsAuthenticated && !string.IsNullOrEmpty(Classes.CurrentUser.FIO))
                {
                    FioLbl.Text = Classes.CurrentUser.FIO;

                    string contactInfo = "";
                    if (!string.IsNullOrEmpty(Classes.CurrentUser.Email)) contactInfo += Classes.CurrentUser.Email;
                    if (!string.IsNullOrEmpty(Classes.CurrentUser.Phone))
                    {
                        if (!string.IsNullOrEmpty(contactInfo)) contactInfo += " | ";
                        contactInfo += Classes.CurrentUser.Phone;
                    }

                    ContactLbl.Text = string.IsNullOrEmpty(contactInfo) ? "Контакты не указаны" : contactInfo;
                }
                else
                {
                    FioLbl.Text = "Пользователь не авторизован";
                    ContactLbl.Text = "";
                }
            }
            LoadYearsAndMonths();
            LoadAllBPRs();
            MonthCB.SelectedItem = DateTime.Now.ToString("MMMM");
            YearCB.SelectedItem = DateTime.Now.Year.ToString();
            UpdateCalendar(null, null);
        }

        private void LoadYearsAndMonths()
        {
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear - 5; i <= currentYear + 5; i++)
                YearCB.Items.Add(i.ToString());
            string[] months = { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
            foreach (var m in months)
                MonthCB.Items.Add(m);
        }

        private void LoadAllBPRs()
        {
            AllBPRs = BPR_info.Select();
            // 🔹 ИСПРАВЛЕНО: Предзагружаем все данные для отображения
            foreach (var bpr in AllBPRs)
            {
                // Принудительно вызываем методы для кэширования данных
                var lesson = bpr.GetLessonName();
                var teacher = bpr.GetResponsibleTeacherName();
                var group = bpr.GetGroupName();
                var room = bpr.GetRoomName();
                var count = bpr.GetActualStudentCount();
            }
        }

        private void UpdateCalendar(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(YearCB.SelectedItem?.ToString(), out int year) ||
                MonthCB.SelectedIndex == -1)
                return;
            int month = MonthCB.SelectedIndex + 1;
            DaysIC.ItemsSource = GenerateDays(year, month);
        }

        private List<DayInfo> GenerateDays(int year, int month)
        {
            var result = new List<DayInfo>();
            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int dayOfWeek = (int)firstDay.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;
            for (int i = 1; i < dayOfWeek; i++)
                result.Add(new DayInfo { Day = "", HasEvent = false });

            for (int day = 1; day <= daysInMonth; day++)
            {
                string dateStr = $"{day:D2}.{month:D2}.{year}";

                // 🔹 Фильтрация: Мои ВПР или Все ВПР
                var dayBPRs = AllBPRs.Where(b => b.Date == dateStr).ToList();

                if (_showMyOnly && Classes.CurrentUser.IsAuthenticated)
                {
                    dayBPRs = dayBPRs.Where(b => b.Responsible_user == Classes.CurrentUser.UserId).ToList();
                }

                // 🔹 Подготавливаем данные для отображения
                foreach (var bpr in dayBPRs)
                {
                    // Убеждаемся, что данные загружены
                    bpr.GetLessonName();
                    bpr.GetResponsibleTeacherName();
                    bpr.GetGroupName();
                    bpr.GetRoomName();
                    bpr.GetActualStudentCount();
                }

                result.Add(new DayInfo
                {
                    Day = day.ToString(),
                    HasEvent = dayBPRs.Any(),
                    BPRList = dayBPRs
                });
            }
            return result;
        }

        private void ToggleMyBPR(object sender, RoutedEventArgs e)
        {
            _showMyOnly = !_showMyOnly;
            myBPRBtn.Content = _showMyOnly ? "Мои ВПР" : "Все ВПР";
            UpdateCalendar(null, null);
        }

        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
        private void OpenRaspisanie_BPR(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Raspisanie_BPR());
        private void OpenClosed_BRP(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Closed_BRP.Closed_BRP());
        private void OpenGroups(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Groups.Groups());
        private void OpenDepartments(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
        private void OpenAuthorization(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenExport(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Export.Export());
        private void OpenLessons(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
        private void OpenKabinets(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Kabinets.Kabinets());

        private void OpenNotificationSettings(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Classes.CurrentUser.Email))
            {
                System.Windows.MessageBox.Show("У вас не указан Email. Уведомления недоступны.", "Внимание", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }
            MainWindow.init.frame.Navigate(new Pages.NotificationSettings(Classes.CurrentUser.Email));
        }
    }
}