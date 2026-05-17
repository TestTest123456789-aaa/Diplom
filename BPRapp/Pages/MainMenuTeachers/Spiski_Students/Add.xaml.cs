using BPRapp.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // ← Добавлено для BrushConverter

namespace BPRapp.Pages.MainMenuTeachers.Spiski_Students
{
    public partial class Add : Page
    {
        private List<Classes.Groups> AllGroups = Classes.Groups.Select();
        private Student_Info student_Info;
        private int? bprId;

        // 🔹 Вспомогательный метод для получения кисти из HEX-цвета
        private Brush GetBrushFromHex(string hexColor)
        {
            try
            {
                return (Brush)new BrushConverter().ConvertFromString(hexColor);
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public Add(Student_Info student_Info = null, int? bprId = null)
        {
            InitializeComponent();
            this.student_Info = student_Info;
            this.bprId = bprId;

            // Инициализация ComboBox групп
            var groups = Classes.Groups.Select();
            group_nameTB.ItemsSource = groups;
            group_nameTB.DisplayMemberPath = "Name";
            group_nameTB.SelectedValuePath = "Id";

            // Инициализация ComboBox студентов
            var allStudents = Student_Info.Select();
            studentCB.ItemsSource = allStudents;
            studentCB.DisplayMemberPath = "FIO";
            studentCB.SelectedValuePath = "Id";

            // Если редактируем существующую запись
            if (student_Info != null)
            {
                nomer_paketaTB.Text = student_Info.Nomer_paketa ?? "";
                kodTB.Text = student_Info.Kod ?? "";
                studentCB.SelectedValue = student_Info.Id;
                otmetka_ov_otsytstviiTB.Text = student_Info.Otmetka_ov_otsytstvii ?? "";
                sredniy_ball_attestataTB.Text = student_Info.Sredniy_ball_attestata ?? "";
                otsenka_po_russkomy_yazikyTB.Text = student_Info.Otsenka_po_russkomy_yaziky ?? "";
                otsenka_po_matematikeTB.Text = student_Info.Otsenka_po_matematike ?? "";

                if (student_Info.Pol == "Мужской")
                    polTB.SelectedIndex = 0;
                else if (student_Info.Pol == "Женский")
                    polTB.SelectedIndex = 1;

                if (student_Info.Forma_obychenya == "Очная")
                    forma_obychenyaTB.SelectedIndex = 0;
                else if (student_Info.Forma_obychenya == "Заочная")
                    forma_obychenyaTB.SelectedIndex = 1;

                group_nameTB.SelectedValue = student_Info.Group_name;
                BthAdd.Content = "💾 Изменить запись";
            }
            else
            {
                if (bprId.HasValue)
                {
                    var bpr = BPR_info.Select().FirstOrDefault(b => b.Id == bprId.Value);
                    if (bpr?.GroupId.HasValue == true)
                    {
                        group_nameTB.SelectedValue = bpr.GroupId.Value;
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
            try
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
                    studentCB.Focus();
                    return;
                }

                string nomerPaketa = nomer_paketaTB.Text?.Trim() ?? "";
                string kod = kodTB.Text?.Trim() ?? "";

                // 🔹 Валидация 2: ПРОВЕРКА НА УНИКАЛЬНОСТЬ №пакета в рамках одного ВПР
                if (!string.IsNullOrWhiteSpace(nomerPaketa))
                {
                    var allStudentsInBPR = Student_Info.Select()
                        .Where(s => s.Spisok_BPR_Id == bprId)
                        .ToList();

                    // Проверяем, не используется ли этот №пакета другим студентом
                    var duplicatePackage = allStudentsInBPR.FirstOrDefault(s =>
                        s.Id != (student_Info?.Id ?? 0) && // Исключаем текущего студента при редактировании
                        !string.IsNullOrWhiteSpace(s.Nomer_paketa) &&
                        s.Nomer_paketa.ToLower() == nomerPaketa.ToLower());

                    if (duplicatePackage != null)
                    {
                        MessageBox.Show(
                            $"⚠️ Номер пакета \"{nomerPaketa}\" уже используется!\n\n" +
                            $"Студент: {duplicatePackage.FIO}\n\n" +
                            "Пожалуйста, введите уникальный номер пакета.",
                            "Дублирование номера пакета",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        nomer_paketaTB.BorderBrush = Brushes.Red;
                        nomer_paketaTB.Focus();
                        return;
                    }
                }

                // 🔹 Валидация 3: ПРОВЕРКА НА УНИКАЛЬНОСТЬ Кода в рамках одного ВПР
                if (!string.IsNullOrWhiteSpace(kod))
                {
                    var allStudentsInBPR = Student_Info.Select()
                        .Where(s => s.Spisok_BPR_Id == bprId)
                        .ToList();

                    // Проверяем, не используется ли этот Код другим студентом
                    var duplicateCode = allStudentsInBPR.FirstOrDefault(s =>
                        s.Id != (student_Info?.Id ?? 0) && // Исключаем текущего студента при редактировании
                        !string.IsNullOrWhiteSpace(s.Kod) &&
                        s.Kod.ToLower() == kod.ToLower());

                    if (duplicateCode != null)
                    {
                        MessageBox.Show(
                            $"⚠️ Код студента \"{kod}\" уже используется!\n\n" +
                            $"Студент: {duplicateCode.FIO}\n\n" +
                            "Пожалуйста, введите уникальный код.",
                            "Дублирование кода студента",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        kodTB.BorderBrush = Brushes.Red;
                        kodTB.Focus();
                        return;
                    }
                }

                // Сбрасываем подсветку ошибок
                var defaultBrush = GetBrushFromHex("#C5D5E5");
                nomer_paketaTB.BorderBrush = defaultBrush;
                kodTB.BorderBrush = defaultBrush;

                if (student_Info == null)
                {
                    var existingStudent = Student_Info.Select()
                        .FirstOrDefault(s => s.FIO.ToLower() == fio.ToLower() &&
                                            s.Spisok_BPR_Id == bprId);

                    if (existingStudent != null)
                    {
                        MessageBox.Show(
                            $"⚠️ Студент \"{fio}\" уже добавлен в этот список ВПР.\n\n" +
                            "Если нужно изменить данные - выберите его из списка для редактирования.",
                            "Студент уже существует",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    var newStudent = new Student_Info(
                        0, nomerPaketa, pol, kod, fio,
                        otmetka_ov_otsytstviiTB.Text, sredniy_ball_attestataTB.Text,
                        otsenka_po_russkomy_yazikyTB.Text, otsenka_po_matematikeTB.Text,
                        forma, groupId, bprId
                    );

                    newStudent.Add();
                    MessageBox.Show("✅ Запись создана", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    student_Info.Nomer_paketa = nomerPaketa;
                    student_Info.Kod = kod;
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
                    MessageBox.Show("✅ Данные обновлены", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                MainWindow.init.frame.Navigate(new Spiski_Students(bprId ?? 0));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"❌ Ошибка при сохранении:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ResetForm(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Очистить все поля формы?\n" +
                "⚠️ Данные о группе будут сохранены.",
                "Подтверждение сброса",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                nomer_paketaTB.Clear();
                kodTB.Clear();
                otsenka_po_russkomy_yazikyTB.Clear();
                otsenka_po_matematikeTB.Clear();
                sredniy_ball_attestataTB.Clear();

                polTB.SelectedIndex = -1;
                forma_obychenyaTB.SelectedIndex = -1;
                otmetka_ov_otsytstviiTB.SelectedIndex = -1;
                studentCB.Text = "";
                studentCB.SelectedIndex = -1;

                // 🔹 Возвращаем стандартные границы (исправлено)
                var defaultBrush = GetBrushFromHex("#C5D5E5");
                nomer_paketaTB.BorderBrush = defaultBrush;
                kodTB.BorderBrush = defaultBrush;

                nomer_paketaTB.Focus();
            }
        }

        private void GoBackSpisok_Students(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Spiski_Students(bprId ?? 0));
    }
}