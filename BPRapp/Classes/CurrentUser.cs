using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPRapp.Classes
{
    public static class CurrentUser
    {
        public static int UserId { get; set; }
        public static string Role { get; set; }
        public static string FIO { get; set; }
        public static string Email { get; set; }
        public static string Phone { get; set; }

        public static void SetUser(int id, string role, string fio, string email = "", string phone = "")
        {
            UserId = id;
            Role = role;
            FIO = fio;
            Email = email;
            Phone = phone;
        }

        public static bool IsAuthenticated => UserId > 0;
    }
}