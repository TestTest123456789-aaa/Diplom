using System;
using System.Collections.Generic;

namespace BPRapp.Classes
{
    public static class DataCache
    {
        public static List<Users> Users { get; private set; } = new List<Users>();
        public static List<Groups> Groups { get; private set; } = new List<Groups>();
        public static List<Lessons> Lessons { get; private set; } = new List<Lessons>();
        public static List<Rooms> Rooms { get; private set; } = new List<Rooms>();
        public static List<Departments> Departments { get; private set; } = new List<Departments>();
        public static List<Specialties> Specialties { get; private set; } = new List<Specialties>();

        public static bool IsLoaded { get; private set; } = false;

        public static void LoadAll()
        {
            if (IsLoaded) return;
            try
            {
                // Используем полные имена, чтобы избежать конфликта с неймспейсами Pages
                Users = BPRapp.Classes.Users.Select();
                Groups = BPRapp.Classes.Groups.Select();
                Lessons = BPRapp.Classes.Lessons.Select();
                Rooms = BPRapp.Classes.Rooms.Select();
                Departments = BPRapp.Classes.Departments.Select();
                Specialties = BPRapp.Classes.Specialties.Select();
                IsLoaded = true;
            }
            catch
            {
                // В случае ошибки кэш сбрасывается, следующий вызов попробует снова
                IsLoaded = false;
            }
        }

        public static void Refresh()
        {
            IsLoaded = false;
            LoadAll();
        }
    }
}