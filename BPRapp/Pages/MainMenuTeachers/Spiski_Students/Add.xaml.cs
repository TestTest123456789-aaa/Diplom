using BPRapp.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Spiski_Students
{
    public partial class Add : Page
    {
        private List<Classes.Groups> AllGroups = Classes.Groups.Select();
        private Student_Info student_Info;
        private int? bprId;

        public Add(Student_Info student_Info = null, int? bprId = null)
        {
            InitializeComponent();
            this.student_Info = student_Info;
            this.bprId = bprId;

            group_nameTB.ItemsSource = AllGroups;
            group_nameTB.DisplayMemberPath = "Name";
            group_nameTB.SelectedValuePath = "Id";

            var allStudents = Student_Info.Select();
            studentCB.ItemsSource = allStudents;

            if (student_Info != null)
            {
                nomer_paketaTB.Text = student_Info.Nomer_paketa ?? "";
                kodTB.Text = student_Info.Kod ?? "";
                studentCB.SelectedValue = student_Info.Id;
                otmetka_ov_otsytstviiTB.Text = student_Info.Otmetka_ov_otsytstvii ?? "";
                sredniy_ball_attestataTB.Text = student_Info.Sredniy_ball_attestata ?? "";
                otsenka_po_russkomy_yazikyTB.Text = student_Info.Otsenka_po_russkomy_yaziky ?? "";
                otsenka_po_matematikeTB.Text = student_Info.Otsenka_po_matematike ?? "";
                polTB.SelectedItem = polTB.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == student_Info.Pol);
                forma_obychenyaTB.SelectedItem = forma_obychenyaTB.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString() == student_Info.Forma_obychenya);
                group_nameTB.SelectedValue = student_Info.Group_name;
                BthAdd.Content = "Изменить запись";
            }
            else
            {
                // При добавлении нового студента в ВПР
                if (bprId.HasValue)
                {
                    var bpr = BPR_info.Select().FirstOrDefault(b => b.Id == bprId.Value);
                    if (bpr?.GroupId.HasValue == true)
                    {
                        group_nameTB.SelectedValue = bpr.GroupId.Value;
                        // Загружаем студентов этой группы
                        var groupStudents = allStudents
                            .Where(s => s.Group_name == bpr.GroupId.Value && s.Spisok_BPR_Id == null)
                            .ToList();
                        studentCB.ItemsSource = groupStudents;
                    }
                }
            }
        }

        private void AddStudent_info(object sender, RoutedEventArgs e)
        {
            string pol = (polTB.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string forma = (forma_obychenyaTB.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            int groupId = group_nameTB.SelectedValue is int gId ? gId : 0;
            string fio = "";
            if (studentCB.SelectedValue != null)
            {
                var selectedStudent = Student_Info.Select()
                    .FirstOrDefault(s => s.Id == (int)studentCB.SelectedValue);
                fio = selectedStudent?.FIO ?? studentCB.Text;
            }
            else
            {
                fio = studentCB.Text.Trim();
            }
            if (string.IsNullOrWhiteSpace(fio))
            {
                MessageBox.Show("Введите ФИО студента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (student_Info == null)
            {
                new Student_Info(
                    0,
                    nomer_paketaTB.Text,
                    pol,
                    kodTB.Text,
                    fio,
                    otmetka_ov_otsytstviiTB.Text,
                    sredniy_ball_attestataTB.Text,
                    otsenka_po_russkomy_yazikyTB.Text,
                    otsenka_po_matematikeTB.Text,
                    forma,
                    groupId,
                    bprId
                ).Add();
                MessageBox.Show("Запись создана");
            }
            else
            {
                student_Info.Nomer_paketa = nomer_paketaTB.Text;
                student_Info.Kod = kodTB.Text;
                student_Info.FIO = fio;
                student_Info.Otmetka_ov_otsytstvii = otmetka_ov_otsytstviiTB.Text;
                student_Info.Sredniy_ball_attestata = sredniy_ball_attestataTB.Text;
                student_Info.Otsenka_po_russkomy_yaziky = otsenka_po_russkomy_yazikyTB.Text;
                student_Info.Otsenka_po_matematike = otsenka_po_matematikeTB.Text;
                student_Info.Pol = pol;
                student_Info.Forma_obychenya = forma;
                student_Info.Group_name = groupId;
                student_Info.Spisok_BPR_Id = bprId ?? student_Info.Spisok_BPR_Id;
                student_Info.Update();
                MessageBox.Show("Данные обновлены");
            }
            MainWindow.init.frame.Navigate(new Spiski_Students(bprId ?? 0));
        }

        private void GoBackSpisok_Students(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Spiski_Students(bprId ?? 0));
    }
}