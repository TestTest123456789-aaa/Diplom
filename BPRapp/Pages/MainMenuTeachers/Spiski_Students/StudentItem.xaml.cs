using BPRapp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPRapp.Pages.MainMenuTeachers.Spiski_Students
{
    public partial class StudentItem : UserControl
    {
        public StudentItem(Student_Info student)
        {
            InitializeComponent();
            fioTB.Text = student.FIO;
            polTB.Text = student.Pol;
            formaTB.Text = student.Forma_obychenya;

            var group = Classes.Groups.Select().FirstOrDefault(g => g.Id == student.Group_name);
            groupTB.Text = group?.Name ?? "—";
        }
    }
}