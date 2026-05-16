using BPRapp.Classes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Spiski_Students
{
    public partial class Spiski_Students : Page
    {
        private int currentBprId;

        public Spiski_Students(int bprId)
        {
            InitializeComponent();
            currentBprId = bprId;
            Loaded += Spiski_Students_Loaded;
        }

        private void Spiski_Students_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Spiski_StudentParent.Children.Clear();
                var bpr = BPR_info.Select().FirstOrDefault(b => b.Id == currentBprId);

                if (bpr == null)
                {
                    MessageBox.Show("Ошибка: ВПР не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!bpr.GroupId.HasValue)
                {
                    MessageBox.Show("Для данного ВПР не выбрана группа. Пожалуйста, выберите группу в настройках ВПР.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var allStudents = Student_Info.Select();
                var vprStudents = allStudents.Where(s => s.Spisok_BPR_Id == currentBprId).ToList();

                if (vprStudents.Count == 0)
                {
                    var masterStudents = allStudents
                        .Where(s => s.Group_name == bpr.GroupId.Value && s.Spisok_BPR_Id == null)
                        .ToList();

                    foreach (var s in masterStudents)
                    {
                        var existingCopy = allStudents.FirstOrDefault(cs =>
                            cs.FIO.Equals(s.FIO, StringComparison.OrdinalIgnoreCase) &&
                            cs.Group_name == s.Group_name &&
                            cs.Spisok_BPR_Id == currentBprId);

                        if (existingCopy == null)
                        {
                            new Student_Info(
                                0, "", s.Pol, "", s.FIO, "", "", "", "", s.Forma_obychenya, bpr.GroupId.Value, currentBprId
                            ).Add();
                        }
                    }

                    vprStudents = Student_Info.Select().Where(s => s.Spisok_BPR_Id == currentBprId).ToList();
                    if (vprStudents.Count > 0)
                        MessageBox.Show($"✅ Автоматически загружено {vprStudents.Count} студентов из группы.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                foreach (var student in vprStudents.OrderBy(s => s.FIO))
                {
                    Spiski_StudentParent.Children.Add(new Item(student, currentBprId));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка загрузки списка: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"[Spiski_Students_Loaded ERROR]: {ex}");
            }
        }

        private void AddSpisok_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Add(null, currentBprId));

        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
    }
}