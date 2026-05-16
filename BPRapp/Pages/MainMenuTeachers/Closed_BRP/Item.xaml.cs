using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BPRapp.Pages.MainMenuTeachers.Closed_BRP
{
    public partial class Item : UserControl
    {
        List<Classes.Lessons> AllLessons = Classes.Lessons.Select();
        List<Classes.Users> AllTeachers = Classes.Users.Select();
        private BPR_info bpr;
        private DispatcherTimer _timer;

        public Item(BPR_info bpr_info)
        {
            InitializeComponent();
            this.bpr = bpr_info;

            dateTB.Text = bpr_info.Date;
            timeTB.Text = bpr_info.Time;
            countTB.Text = bpr_info.GetActualStudentCount().ToString();
            responsible_userTB.Text = AllTeachers.Find(x => x.Id == bpr_info.Responsible_user)?.FIO ?? "Не назначен";
            lessonTB.Text = AllLessons.Find(x => x.Id == bpr_info.Lesson)?.Lesson ?? "Не указан";

            if (TryGetDateTime(bpr.Date, bpr.Time, out DateTime eventTime))
            {
                UpdateCountdown(eventTime);
                StartTimer(eventTime);
            }
            else
            {
                ClosedDate.Content = "⚠️ Неверная дата или время";
                ClosedDate.Foreground = System.Windows.Media.Brushes.OrangeRed;
            }

            DataContext = bpr_info;
        }

        private void StartTimer(DateTime eventTime)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += (s, e) => UpdateCountdown(eventTime);
            _timer.Start();
        }

        private void UpdateCountdown(DateTime eventTime)
        {
            var now = DateTime.Now;

            if (eventTime <= now)
            {
                ClosedDate.Content = "✅ ВПР уже началось или завершилось!";
                ClosedDate.Foreground = System.Windows.Media.Brushes.Green;
                _timer?.Stop();
                return;
            }

            var timeUntil = eventTime - now;
            string countdownText = FormatTimeSpan(timeUntil);

            ClosedDate.Content = $"⏰ ВПР начнётся через: {countdownText}";
            ClosedDate.Foreground = System.Windows.Media.Brushes.CadetBlue;
        }

        private string FormatTimeSpan(TimeSpan span)
        {
            List<string> parts = new List<string>();

            if (span.Days > 0)
                parts.Add($"{span.Days} {(span.Days == 1 ? "день" : span.Days < 5 ? "дня" : "дней")}");

            if (span.Hours > 0)
                parts.Add($"{span.Hours} {(span.Hours == 1 ? "час" : span.Hours < 5 ? "часа" : "часов")}");

            if (span.Minutes > 0)
                parts.Add($"{span.Minutes} {(span.Minutes == 1 ? "минута" : span.Minutes < 5 ? "минуты" : "минут")}");

            if (parts.Count == 0)
                return "меньше минуты";

            return string.Join(" ", parts);
        }

        private bool TryGetDateTime(string dateStr, string timeStr, out DateTime result)
        {
            try
            {
                if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var datePart))
                {
                    string startTime = timeStr.Contains("-") ?
                        timeStr.Split('-')[0].Trim() : timeStr.Trim();

                    if (TimeSpan.TryParse(startTime, out var timeSpan))
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

        public void Unload()
        {
            _timer?.Stop();
        }
    }
}