using BPRapp.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnitTests
{
    [TestClass]
    public class BPR_CoreLogicTests
    {
        // Сбрасываем статическое состояние CurrentUser перед каждым тестом
        [TestInitialize]
        public void Setup()
        {
            CurrentUser.SetUser(0, "", "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            CurrentUser.SetUser(0, "", "");
        }

        //Тест 1: Генерация кода восстановления пароля
        [TestMethod]
        public void EmailService_GenerateCode_ShouldReturnValidSixDigitString()
        {
            string code = EmailService.GenerateCode();

            Assert.IsNotNull(code, "Код не должен быть null");
            Assert.AreEqual(6, code.Length, "Код должен состоять ровно из 6 символов");
            Assert.IsTrue(int.TryParse(code, out int numericValue), "Код должен содержать только цифры");
            Assert.IsTrue(numericValue >= 100000 && numericValue <= 999999, "Код должен быть в диапазоне [100000; 999999]");
        }

        //Тест 2: Логика авторизации текущего пользователя
        [TestMethod]
        public void CurrentUser_AuthenticationState_ShouldUpdateCorrectly()
        {
            Assert.IsFalse(CurrentUser.IsAuthenticated);
            CurrentUser.SetUser(42, "Преподаватель", "Иванов И.И.");
            Assert.IsTrue(CurrentUser.IsAuthenticated);
            Assert.AreEqual(42, CurrentUser.UserId);
            Assert.AreEqual("Преподаватель", CurrentUser.Role);
            Assert.AreEqual("Иванов И.И.", CurrentUser.FIO);
            CurrentUser.SetUser(0, "", "");
            Assert.IsFalse(CurrentUser.IsAuthenticated);
        }

        //Тест 3: Форматирование обратного отсчёта (русская локаль)
        [TestMethod]
        public void TimeFormatter_FormatTimeSpan_ShouldHandleRussianPluralization()
        {
            Assert.AreEqual("1 день 2 часа 30 минут", FormatTimeSpanHelper(new TimeSpan(1, 2, 30, 0)));
            Assert.AreEqual("5 дней 10 часов", FormatTimeSpanHelper(new TimeSpan(5, 10, 0, 0)));
            Assert.AreEqual("1 час 1 минута", FormatTimeSpanHelper(new TimeSpan(0, 1, 1, 0)));
            Assert.AreEqual("меньше минуты", FormatTimeSpanHelper(new TimeSpan(0, 0, 30, 0)));
        }

        //Тест 4: Парсинг даты и времени ВПР (извлечение времени начала)
        [TestMethod]
        public void DateTimeParser_TryParseBPRDateTime_ShouldExtractStartTimeCorrectly()
        {
            string validDate = "19.05.2026";
            string validTimeRange = "09:05 - 10:30";
            string invalidDate = "2026-05-19";

            bool success1 = TryParseBPRDateTimeHelper(validDate, validTimeRange, out DateTime dt1);
            bool success2 = TryParseBPRDateTimeHelper(invalidDate, validTimeRange, out DateTime dt2);

            Assert.IsTrue(success1);
            Assert.AreEqual(new DateTime(2026, 5, 19, 9, 5, 0), dt1);
            Assert.IsFalse(success2);
            Assert.AreEqual(DateTime.MinValue, dt2);
        }

        #region Вспомогательные методы (дублируют логику из UI-слоя для изоляции)
        private static string FormatTimeSpanHelper(TimeSpan span)
        {
            var parts = new List<string>();
            if (span.Days > 0) parts.Add($"{span.Days} {(span.Days == 1 ? "день" : span.Days < 5 ? "дня" : "дней")}");
            if (span.Hours > 0) parts.Add($"{span.Hours} {(span.Hours == 1 ? "час" : span.Hours < 5 ? "часа" : "часов")}");
            if (span.Minutes > 0) parts.Add($"{span.Minutes} {(span.Minutes == 1 ? "минута" : span.Minutes < 5 ? "минуты" : "минут")}");
            return parts.Count == 0 ? "меньше минуты" : string.Join(" ", parts);
        }

        private static bool TryParseBPRDateTimeHelper(string dateStr, string timeStr, out DateTime result)
        {
            result = DateTime.MinValue;
            try
            {
                if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var datePart))
                {
                    string startTime = timeStr.Contains("-") ? timeStr.Split('-')[0].Trim() : timeStr.Trim();
                    if (TimeSpan.TryParse(startTime, out var timeSpan))
                    {
                        result = datePart.Date + timeSpan;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
        #endregion
    }
}