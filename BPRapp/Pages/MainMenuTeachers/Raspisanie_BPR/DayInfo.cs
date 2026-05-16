using BPRapp.Classes;
using System.Collections.Generic;

namespace BPRapp.Pages.MainMenuTeachers.Raspisanie_BPR
{
    public class DayInfo
    {
        public string Day { get; set; }
        public bool HasEvent { get; set; }
        public List<BPR_info> BPRList { get; set; } = new List<BPR_info>();
    }
}