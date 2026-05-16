using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Spiski_BPR
{
    public partial class Spiski_BPR : Page
    {
        private List<BPR_info> AllBPR = new List<BPR_info>();
        private bool _showMyOnly = false;

        public Spiski_BPR()
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
            ApplyFilters();
        }

        private void LoadData()
        {
            AllBPR = BPR_info.Select();
            var groups = Classes.Groups.Select();
            groupFilterCB.Items.Clear();
            groupFilterCB.Items.Add(new Classes.Groups(0, "Все группы"));
            foreach (var g in groups) groupFilterCB.Items.Add(g);
            groupFilterCB.SelectedIndex = 0;
            var lessons = Classes.Lessons.Select();
            lessonFilterCB.Items.Clear();
            lessonFilterCB.Items.Add(new Classes.Lessons(0, "Все предметы"));
            foreach (var l in lessons) lessonFilterCB.Items.Add(l);
            lessonFilterCB.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filtered = AllBPR.AsEnumerable();
            if (groupFilterCB.SelectedValue is int groupId && groupId > 0)
                filtered = filtered.Where(b => b.GroupId == groupId);
            if (lessonFilterCB.SelectedValue is int lessonId && lessonId > 0)
                filtered = filtered.Where(b => b.Lesson == lessonId);
            if (_showMyOnly && Classes.CurrentUser.IsAuthenticated)
                filtered = filtered.Where(b => b.Responsible_user == Classes.CurrentUser.UserId);

            var sorted = filtered
                .OrderBy(b =>
                {
                    string t = b.Time.Contains("-") ? b.Time.Split('-')[0].Trim() : b.Time.Trim();
                    DateTime.TryParse($"{b.Date} {t}", out var dt);
                    return dt;
                })
                .ToList();
            Spiski_BPRParent.Children.Clear();
            foreach (var bpr in sorted)
                Spiski_BPRParent.Children.Add(new Item(bpr));
        }

        private void ToggleMyBPR(object sender, RoutedEventArgs e)
        {
            _showMyOnly = !_showMyOnly;
            myBPRBtn.Content = _showMyOnly ? "Все ВПР" : "Мои ВПР";
            ApplyFilters();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Spiski_BPR());
        private void OpenRaspisanie_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Raspisanie_BPR.Raspisanie_BPR());
        private void OpenClosed_BRP(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Closed_BRP.Closed_BRP());
        private void OpenGroups(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Groups.Groups());
        private void OpenDepartments(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
        private void OpenAuthorization(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenExport(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Export.Export());
        private void OpenLessons(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
        private void AddSpisok_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Add());
        private void OpenKabinets(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Kabinets.Kabinets());
    }
}