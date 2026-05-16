using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Departments
{
    public partial class Add : Page
    {
        Classes.Departments department;
        List<Classes.Users> AllTeachers;

        public Add(Classes.Departments department = null)
        {
            InitializeComponent();
            AllTeachers = Classes.Users.Select().Where(u => u.Role == "Преподаватель").ToList();

            var teacherList = new List<Classes.Users>
            {
                new Classes.Users(0, "Не назначен", "", "", "", "", "")
            };
            teacherList.AddRange(AllTeachers);
            headTeacherCB.ItemsSource = teacherList;
            headTeacherCB.DisplayMemberPath = "FIO";
            headTeacherCB.SelectedValuePath = "Id";

            if (department != null)
            {
                this.department = department;
                nameTB.Text = department.Name;

                if (department.HeadTeacherId.HasValue)
                {
                    headTeacherCB.SelectedValue = department.HeadTeacherId.Value;
                }
                else
                {
                    headTeacherCB.SelectedIndex = 0;
                }
                BthAdd.Content = "Изменить информацию";
            }
        }

        private void AddDepartment(object sender, RoutedEventArgs e)
        {
            int? headTeacherId = null;
            if (headTeacherCB.SelectedValue is int selectedId && selectedId > 0)
            {
                headTeacherId = selectedId;
            }

            if (this.department == null)
            {
                Classes.Departments newDept = new Classes.Departments(0, nameTB.Text, headTeacherId);
                newDept.Add();
                MessageBox.Show("Отделение добавлено");
            }
            else
            {
                Classes.Departments newDept = new Classes.Departments(department.Id, nameTB.Text, headTeacherId);
                newDept.Update();
                MessageBox.Show("Информация изменена");
            }
            MainWindow.init.frame.Navigate(new Departments());
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Departments());
        }
    }
}