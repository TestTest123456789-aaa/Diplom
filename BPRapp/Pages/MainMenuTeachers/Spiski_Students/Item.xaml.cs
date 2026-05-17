using BPRapp.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace BPRapp.Pages.MainMenuTeachers.Spiski_Students
{
    public partial class Item : UserControl
    {
        private Student_Info student_Info;
        private int currentBprId;

        // ✅ 1. Пустой конструктор для дизайнера XAML
        public Item()
        {
            InitializeComponent();
        }

        // ✅ 2. Основной конструктор (Fix CS1729)
        public Item(Student_Info student_Info, int bprId) : this()
        {
            this.student_Info = student_Info;
            this.currentBprId = bprId;

            nomer_paketaTB.Text = student_Info.Nomer_paketa ?? "";
            polTB.Text = student_Info.Pol ?? "";
            kodTB.Text = student_Info.Kod ?? "";
            FIOTB.Text = student_Info.FIO ?? "";
            otmetka_ov_otsytstviiTB.Text = student_Info.Otmetka_ov_otsytstvii ?? "";
            sredniy_ball_attestataTB.Text = student_Info.Sredniy_ball_attestata ?? "";
            otsenka_po_russkomy_yazikyTB.Text = student_Info.Otsenka_po_russkomy_yaziky ?? "";
            otsenka_po_matematikeTB.Text = student_Info.Otsenka_po_matematike ?? "";
            forma_obychenyaTB.Text = student_Info.Forma_obychenya ?? "";

            var group = Classes.Groups.Select().FirstOrDefault(g => g.Id == student_Info.Group_name);
            group_nameTB.Text = group?.Name ?? "";
        }

        private void EditStudent(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Add(student_Info, currentBprId));

        private void DeleteStudent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить студента из этого списка ВПР?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                student_Info.Delete();
                MainWindow.init.frame.Navigate(new Spiski_Students(currentBprId));
            }
        }
    }
}