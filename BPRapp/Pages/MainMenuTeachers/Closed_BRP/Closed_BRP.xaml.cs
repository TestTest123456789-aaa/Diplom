using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Closed_BRP
{
    public partial class Closed_BRP : Page
    {
        private bool _showMyOnly = false;
        private List<Classes.BPR_info> AllBPR_info = new List<Classes.BPR_info>();
        private List<Classes.Users> AllTeachers = new List<Classes.Users>();
        private List<Classes.Lessons> AllLessons = new List<Classes.Lessons>();
        private List<Classes.Groups> AllGroups = new List<Classes.Groups>();

        public Closed_BRP()
        {
            InitializeComponent();
            Loaded += Spiski_BPR_Loaded;
        }

        private void Spiski_BPR_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Auth: {Classes.CurrentUser.IsAuthenticated}, FIO: '{Classes.CurrentUser.FIO}', UserId: {Classes.CurrentUser.UserId}");
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
            LoadData();
            SetupFilters();
            ApplyFilters();
        }

        private void LoadData()
        {
            AllBPR_info = Classes.BPR_info.Select();
            AllTeachers = Classes.Users.Select().Where(u => u.Role == "Преподаватель").ToList();
            AllLessons = Classes.Lessons.Select();
            AllGroups = Classes.Groups.Select();
        }

        private void SetupFilters()
        {
            teacherFilterCB.Items.Clear();
            teacherFilterCB.Items.Add(new Classes.Users(0, "Все преподаватели", "", "", "", "", ""));
            foreach (var t in AllTeachers)
                teacherFilterCB.Items.Add(t);
            teacherFilterCB.SelectedIndex = 0;
            teacherFilterCB.SelectionChanged += (s, e) => ApplyFilters();

            lessonFilterCB.Items.Clear();
            lessonFilterCB.Items.Add(new Classes.Lessons(0, "Все предметы"));
            foreach (var l in AllLessons)
                lessonFilterCB.Items.Add(l);
            lessonFilterCB.SelectedIndex = 0;
            lessonFilterCB.SelectionChanged += (s, e) => ApplyFilters();

            groupFilterCB.Items.Clear();
            groupFilterCB.Items.Add(new Classes.Groups(0, "Все группы", null, null, null));
            foreach (var g in AllGroups)
                groupFilterCB.Items.Add(g);
            groupFilterCB.SelectedIndex = 0;
            groupFilterCB.SelectionChanged += (s, e) => ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = AllBPR_info.AsEnumerable();
            if (teacherFilterCB.SelectedValue is int teacherId && teacherId > 0)
                filtered = filtered.Where(b => b.Responsible_user == teacherId);
            if (lessonFilterCB.SelectedValue is int lessonId && lessonId > 0)
                filtered = filtered.Where(b => b.Lesson == lessonId);
            if (groupFilterCB.SelectedValue is int groupId && groupId > 0)
                filtered = filtered.Where(b => b.GroupId == groupId);
            if (_showMyOnly && Classes.CurrentUser.IsAuthenticated)
                filtered = filtered.Where(b => b.Responsible_user == Classes.CurrentUser.UserId);
            var sorted = filtered
                .OrderBy(b =>
                {
                    if (TryParseBPRDateTime(b, out var dt))
                        return dt;
                    return DateTime.MaxValue;
                }).ToList();
            Closed_BPRParent.Children.Clear();
            foreach (var bpr in sorted)
                Closed_BPRParent.Children.Add(new Item(bpr));
        }

        private bool TryParseBPRDateTime(BPR_info bpr, out DateTime result)
        {
            try
            {
                if (DateTime.TryParseExact(bpr.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var datePart))
                {
                    string timeStr = bpr.Time.Contains("-") ? bpr.Time.Split('-')[0].Trim() : bpr.Time.Trim();
                    if (TimeSpan.TryParse(timeStr, out var timeSpan))
                    {
                        result = datePart.Date + timeSpan;
                        return true;
                    }
                }
            }
            catch
            {

            }

            result = DateTime.MinValue;
            return false;
        }

        private DateTime GetEventTime(BPR_info bpr)
        {
            TryParseBPRDateTime(bpr, out var dt);
            return dt;
        }

        private void ToggleMyBPR(object sender, RoutedEventArgs e)
        {
            _showMyOnly = !_showMyOnly;
            myBPRBtn.Content = _showMyOnly ? "Все ВПР" : "Мои ВПР";
            ApplyFilters();
        }

        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
        private void OpenRaspisanie_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Raspisanie_BPR.Raspisanie_BPR());
        private void OpenClosed_BRP(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Closed_BRP.Closed_BRP());
        private void OpenGroups(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Groups.Groups());
        private void OpenDepartments(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
        private void OpenAuthorization(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenExport(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Export.Export());
        private void OpenLessons(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
        private void OpenKabinets(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Kabinets.Kabinets());
    }
}