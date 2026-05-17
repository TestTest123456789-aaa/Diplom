using BPRapp.Classes;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PdfSharp.Drawing;

namespace BPRapp.Pages.MainMenuTeachers.Export
{
    public partial class Export : Page
    {
        private List<BPR_info> AllBPR = new List<BPR_info>();
        private List<Classes.Groups> AllGroups = new List<Classes.Groups>();

        public Export()
        {
            InitializeComponent();
            Loaded += Export_Loaded;
        }

        private void Export_Loaded(object sender, RoutedEventArgs e)
        {
            if (FioLbl != null)
            {
                if (Classes.CurrentUser.IsAuthenticated && !string.IsNullOrEmpty(Classes.CurrentUser.FIO))
                {
                    FioLbl.Text = Classes.CurrentUser.FIO;

                    string contactInfo = "";
                    if (!string.IsNullOrEmpty(Classes.CurrentUser.Email)) contactInfo += Classes.CurrentUser.Email;
                    if (!string.IsNullOrEmpty(Classes.CurrentUser.Phone))
                    {
                        if (!string.IsNullOrEmpty(contactInfo)) contactInfo += " | ";
                        contactInfo += Classes.CurrentUser.Phone;
                    }

                    ContactLbl.Text = string.IsNullOrEmpty(contactInfo) ? "Контакты не указаны" : contactInfo;
                }
                else
                {
                    FioLbl.Text = "Пользователь не авторизован";
                    ContactLbl.Text = "";
                }
            }
            LoadData();
        }

        private void LoadData()
        {
            AllBPR = BPR_info.Select();
            AllGroups = Classes.Groups.Select();
            bprListCB.ItemsSource = null;
            bprListCB.ItemsSource = AllBPR;
            bprListCB.SelectedValuePath = "Id";

            var groupsWithAll = new List<Classes.Groups> { new Classes.Groups(0, "Все группы", null, null, null) };
            groupsWithAll.AddRange(AllGroups);

            groupExportCB.Items.Clear();
            groupExportCB.ItemsSource = groupsWithAll;
            groupExportCB.DisplayMemberPath = "Name";
            groupExportCB.SelectedValuePath = "Id";
            groupExportCB.SelectedIndex = 0;

            importGroupCB.ItemsSource = AllGroups;
            importGroupCB.DisplayMemberPath = "Name";
            importGroupCB.SelectedValuePath = "Id";

            bprExportTypeCB.SelectionChanged += (s, args) =>
            {
                if (bprExportTypeCB.SelectedIndex == 2)
                    bprListCB.Visibility = Visibility.Visible;
                else
                    bprListCB.Visibility = Visibility.Collapsed;
            };
        }

        private List<BPR_info> GetSelectedBPR()
        {
            if (bprExportTypeCB.SelectedIndex == 0) return AllBPR;
            if (bprExportTypeCB.SelectedIndex == 1)
                return AllBPR.Where(b => b.Responsible_user == Classes.CurrentUser.UserId).ToList();
            if (bprExportTypeCB.SelectedIndex == 2 && bprListCB.SelectedValue is int id)
                return AllBPR.Where(b => b.Id == id).ToList();
            return new List<BPR_info>();
        }

        private void ExportBPRToExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                var bprToExport = GetSelectedBPR();
                if (!bprToExport.Any())
                {
                    MessageBox.Show("Нет данных для экспорта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"ВПР_Экспорт_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
                if (saveDialog.ShowDialog() != true) return;

                using (var workbook = new XLWorkbook())
                {
                    int sheetIndex = 1;
                    foreach (var bpr in bprToExport)
                    {
                        var ws = workbook.Worksheets.Add($"ВПР_{sheetIndex++}");
                        ws.Cell(1, 1).Value = $"ВПР от {bpr.Date} {bpr.Time}";
                        ws.Cell(1, 1).Style.Font.Bold = true;
                        ws.Cell(1, 1).Style.Font.FontSize = 14;
                        ws.Range(1, 1, 1, 10).Merge();
                        ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell(3, 1).Value = "Предмет:"; ws.Cell(3, 2).Value = bpr.GetLessonName();
                        ws.Cell(4, 1).Value = "Группа:"; ws.Cell(4, 2).Value = bpr.GetGroupName();
                        ws.Cell(5, 1).Value = "Преподаватель:"; ws.Cell(5, 2).Value = bpr.GetResponsibleTeacherName();
                        ws.Cell(6, 1).Value = "Кабинет:"; ws.Cell(6, 2).Value = bpr.GetRoomName();

                        string[] headers = { "№", "ФИО", "Пол", "Код", "№ Пакета", "Отметка", "Ср. балл", "Русский", "Математика", "Форма" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            ws.Cell(8, i + 1).Value = headers[i];
                            ws.Cell(8, i + 1).Style.Font.Bold = true;
                            ws.Cell(8, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                            ws.Cell(8, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        var students = Student_Info.Select()
                            .Where(s => s.Spisok_BPR_Id == bpr.Id)
                            .OrderBy(s => s.FIO)
                            .ToList();

                        int row = 9;
                        foreach (var s in students)
                        {
                            ws.Cell(row, 1).Value = row - 8;
                            ws.Cell(row, 2).Value = s.FIO ?? "";
                            ws.Cell(row, 3).Value = s.Pol ?? "";
                            ws.Cell(row, 4).Value = s.Kod ?? "";
                            ws.Cell(row, 5).Value = s.Nomer_paketa ?? "";
                            ws.Cell(row, 6).Value = s.Otmetka_ov_otsytstvii ?? "";
                            ws.Cell(row, 7).Value = s.Sredniy_ball_attestata ?? "";
                            ws.Cell(row, 8).Value = s.Otsenka_po_russkomy_yaziky ?? "";
                            ws.Cell(row, 9).Value = s.Otsenka_po_matematike ?? "";
                            ws.Cell(row, 10).Value = s.Forma_obychenya ?? "";
                            for (int col = 1; col <= 10; col++)
                                ws.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            row++;
                        }
                        ws.Columns().AdjustToContents();
                    }
                    workbook.SaveAs(saveDialog.FileName);
                    statusTB.Text = $"✅ Экспорт выполнен: {bprToExport.Count} ВПР";
                    MessageBox.Show($"Файл сохранён!\nЭкспортировано ВПР: {bprToExport.Count}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                statusTB.Text = $"❌ Ошибка: {ex.Message}";
            }
        }

        private void ExportGroupsToExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                int gid = groupExportCB.SelectedValue is int id ? id : 0;
                var groups = gid == 0 ? AllGroups : AllGroups.Where(g => g.Id == gid).ToList();
                if (!groups.Any()) { MessageBox.Show("Нет групп", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

                var saveDialog = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", FileName = $"Группы_{DateTime.Now:yyyyMMdd}.xlsx" };
                if (saveDialog.ShowDialog() != true) return;

                using (var workbook = new XLWorkbook())
                {
                    foreach (var g in groups)
                    {
                        var ws = workbook.Worksheets.Add(g.Name);
                        ws.Cell(1, 1).Value = $"Группа: {g.Name}"; ws.Cell(1, 1).Style.Font.Bold = true; ws.Range(1, 1, 1, 5).Merge();
                        ws.Cell(3, 1).Value = "Руководитель:"; ws.Cell(3, 2).Value = g.GetHeadTeacherName();
                        ws.Cell(4, 1).Value = "Отделение:"; ws.Cell(4, 2).Value = g.GetDepartmentName();
                        ws.Cell(5, 1).Value = "Специальность:"; ws.Cell(5, 2).Value = g.GetSpecialtyName();
                        ws.Cell(7, 1).Value = "№"; ws.Cell(7, 2).Value = "ФИО"; ws.Cell(7, 3).Value = "Пол";
                        ws.Cell(7, 4).Value = "Форма"; ws.Cell(7, 5).Value = "Код";
                        ws.Range(7, 1, 7, 5).Style.Font.Bold = true; ws.Range(7, 1, 7, 5).Style.Fill.BackgroundColor = XLColor.LightGray;

                        var students = Student_Info.Select().Where(s => s.Group_name == g.Id && s.Spisok_BPR_Id == null).OrderBy(s => s.FIO).ToList();
                        int row = 8;
                        foreach (var s in students)
                        {
                            ws.Cell(row, 1).Value = row - 7; ws.Cell(row, 2).Value = s.FIO ?? ""; ws.Cell(row, 3).Value = s.Pol ?? "";
                            ws.Cell(row, 4).Value = s.Forma_obychenya ?? ""; ws.Cell(row, 5).Value = s.Kod ?? ""; row++;
                        }
                        ws.Columns().AdjustToContents();
                    }
                    workbook.SaveAs(saveDialog.FileName);
                    MessageBox.Show("Экспорт завершён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ExportBPRToPDF(object sender, RoutedEventArgs e)
        {
            try
            {
                var bprToExport = GetSelectedBPR();
                if (!bprToExport.Any())
                {
                    MessageBox.Show("Нет данных для экспорта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf", FileName = $"ВПР_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };
                if (saveDialog.ShowDialog() != true) return;
                using (var document = new PdfSharp.Pdf.PdfDocument())
                {
                    document.Info.Title = "Экспорт ВПР";
                    foreach (var bpr in bprToExport)
                    {
                        var page = document.AddPage();
                        using (var gfx = XGraphics.FromPdfPage(page))
                        {
                            var font = new XFont("Arial", 16, XFontStyleEx.Bold);
                            var normalFont = new XFont("Arial", 10);
                            var boldFont = new XFont("Arial", 8, XFontStyleEx.Bold);
                            double y = 20;
                            gfx.DrawString($"ВПР от {bpr.Date} {bpr.Time}", font, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.Center);
                            y += 40;
                            gfx.DrawString($"Предмет: {bpr.GetLessonName()}", normalFont, XBrushes.Black, 20, y); y += 20;
                            gfx.DrawString($"Группа: {bpr.GetGroupName()}", normalFont, XBrushes.Black, 20, y); y += 20;
                            gfx.DrawString($"Преподаватель: {bpr.GetResponsibleTeacherName()}", normalFont, XBrushes.Black, 20, y); y += 20;
                            gfx.DrawString($"Кабинет: {bpr.GetRoomName()}", normalFont, XBrushes.Black, 20, y); y += 40;
                            var students = Student_Info.Select().Where(s => s.Spisok_BPR_Id == bpr.Id).OrderBy(s => s.FIO).ToList();
                            string[] headers = { "№", "ФИО", "Пол", "Код", "Пакет", "Отметка", "Ср.балл", "Русский", "Матем.", "Форма" };
                            double x = 20;
                            for (int i = 0; i < headers.Length; i++)
                            {
                                gfx.DrawString(headers[i], boldFont, XBrushes.Black, x, y);
                                x += 80;
                            }
                            y += 20;
                            int num = 1;
                            foreach (var s in students)
                            {
                                x = 20;
                                gfx.DrawString(num++.ToString(), normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.FIO ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Pol ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Kod ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Nomer_paketa ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Otmetka_ov_otsytstvii ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Sredniy_ball_attestata ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Otsenka_po_russkomy_yaziky ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Otsenka_po_matematike ?? "", normalFont, XBrushes.Black, x, y); x += 80;
                                gfx.DrawString(s.Forma_obychenya ?? "", normalFont, XBrushes.Black, x, y);
                                y += 18;
                                if (y > page.Height - 30)
                                {
                                    page = document.AddPage();
                                    using (var gfx2 = XGraphics.FromPdfPage(page))
                                    {
                                        y = 20;
                                        x = 20;
                                        for (int i = 0; i < headers.Length; i++)
                                        {
                                            gfx2.DrawString(headers[i], boldFont, XBrushes.Black, x, y);
                                            x += 80;
                                        }
                                        y += 20;
                                    }
                                }
                            }
                        }
                        if (bpr != bprToExport.Last())
                            document.AddPage();
                    }
                    document.Save(saveDialog.FileName);
                }
                MessageBox.Show($"PDF успешно создан!\nЭкспортировано ВПР: {bprToExport.Count}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                statusTB.Text = $"✅ PDF экспорт: {bprToExport.Count} ВПР";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                statusTB.Text = $"❌ Ошибка PDF: {ex.Message}";
            }
        }

        private void ExportGroupsToPDF(object sender, RoutedEventArgs e)
        {
            try
            {
                int gid = groupExportCB.SelectedValue is int id ? id : 0;
                var groups = gid == 0 ? AllGroups : AllGroups.Where(g => g.Id == gid).ToList();
                if (!groups.Any()) return;

                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Группы_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };
                if (saveDialog.ShowDialog() != true) return;

                using (var document = new PdfSharp.Pdf.PdfDocument())
                {
                    document.Info.Title = "Экспорт групп";

                    foreach (var g in groups)
                    {
                        var page = document.AddPage();
                        using (var gfx = XGraphics.FromPdfPage(page))
                        {
                            var titleFont = new XFont("Helvetica", 16, XFontStyleEx.Bold);
                            var normalFont = new XFont("Helvetica", 10);
                            var boldFont = new XFont("Helvetica", 8, XFontStyleEx.Bold);
                            double y = 20;

                            gfx.DrawString($"Группа: {g.Name}", titleFont, XBrushes.Black,
                                new XRect(0, y, page.Width, 25), XStringFormats.Center);
                            y += 35;

                            gfx.DrawString($"Руководитель: {g.GetHeadTeacherName()}", normalFont, XBrushes.Black, 20, y); y += 18;
                            gfx.DrawString($"Отделение: {g.GetDepartmentName()}", normalFont, XBrushes.Black, 20, y); y += 18;
                            gfx.DrawString($"Специальность: {g.GetSpecialtyName()}", normalFont, XBrushes.Black, 20, y); y += 30;

                            string[] headers = { "№", "ФИО", "Пол", "Форма", "Код" };
                            double x = 20;
                            for (int i = 0; i < headers.Length; i++)
                            {
                                gfx.DrawString(headers[i], boldFont, XBrushes.Black, x, y);
                                x += 100;
                            }
                            y += 20;

                            var students = Student_Info.Select().Where(s => s.Group_name == g.Id && s.Spisok_BPR_Id == null).OrderBy(s => s.FIO).ToList();

                            int num = 1;
                            foreach (var s in students)
                            {
                                x = 20;
                                gfx.DrawString(num++.ToString(), normalFont, XBrushes.Black, x, y); x += 100;
                                gfx.DrawString(s.FIO ?? "", normalFont, XBrushes.Black, x, y); x += 100;
                                gfx.DrawString(s.Pol ?? "", normalFont, XBrushes.Black, x, y); x += 100;
                                gfx.DrawString(s.Forma_obychenya ?? "", normalFont, XBrushes.Black, x, y); x += 100;
                                gfx.DrawString(s.Kod ?? "", normalFont, XBrushes.Black, x, y);
                                y += 18;

                                if (y > page.Height - 30)
                                {
                                    page = document.AddPage();
                                    using (var gfx2 = XGraphics.FromPdfPage(page))
                                    {
                                        y = 20;
                                        x = 20;
                                        for (int i = 0; i < headers.Length; i++)
                                        {
                                            gfx2.DrawString(headers[i], boldFont, XBrushes.Black, x, y);
                                            x += 100;
                                        }
                                        y += 20;
                                    }
                                }
                            }
                        }

                        if (g != groups.Last())
                            document.AddPage();
                    }

                    document.Save(saveDialog.FileName);
                }

                MessageBox.Show("PDF сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string errorMsg = $"Ошибка PDF: {ex.Message}";
                if (ex.InnerException != null)
                    errorMsg += $"\n{ex.InnerException.Message}";
                MessageBox.Show(errorMsg, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportStudentsFromExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                if (importGroupCB.SelectedValue == null) { MessageBox.Show("Выберите группу", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
                int gid = (int)importGroupCB.SelectedValue;
                var openDialog = new OpenFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx" };
                if (openDialog.ShowDialog() != true) return;

                int added = 0, skipped = 0;
                using (var wb = new XLWorkbook(openDialog.FileName))
                {
                    foreach (var ws in wb.Worksheets)
                    {
                        int row = 2;
                        while (!ws.Cell(row, 1).IsEmpty())
                        {
                            string fio = ws.Cell(row, 1).GetValue<string>(), pol = ws.Cell(row, 2).GetValue<string>(),
                                   forma = ws.Cell(row, 3).GetValue<string>(), kod = ws.Cell(row, 4).GetValue<string>();
                            if (!string.IsNullOrWhiteSpace(fio))
                            {
                                var exists = Student_Info.Select().FirstOrDefault(s => s.FIO == fio && s.Group_name == gid);
                                if (exists == null)
                                {
                                    var newStudent = new Student_Info(0, "", pol, kod, fio, "", "", "", "", forma, gid, null);
                                    if (newStudent.Add(out string errorMessage))
                                        added++;
                                    else
                                        System.Diagnostics.Debug.WriteLine($"⚠️ Не добавлен: {errorMessage}");
                                }
                                else skipped++;
                            }
                            row++;
                        }
                    }
                }
                importStatusTB.Text = $"✅ Добавлено: {added}, Пропущено: {skipped}";
                MessageBox.Show($"Импорт завершён!\nДобавлено: {added}\nПропущено: {skipped}", "Импорт", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); importStatusTB.Text = $"❌ {ex.Message}"; }
        }

        private void DownloadStudentTemplate(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", FileName = "Шаблон_студенты.xlsx" };
                if (saveDialog.ShowDialog() != true) return;
                using (var wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Студенты");
                    ws.Cell(1, 1).Value = "ФИО"; ws.Cell(1, 2).Value = "Пол"; ws.Cell(1, 3).Value = "Форма"; ws.Cell(1, 4).Value = "Код"; ws.Cell(1, 5).Value = "Примечание";
                    ws.Range(1, 1, 1, 5).Style.Font.Bold = true; ws.Range(1, 1, 1, 5).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell(2, 1).Value = "Иванов Иван Иванович"; ws.Cell(2, 2).Value = "Мужской"; ws.Cell(2, 3).Value = "Очная"; ws.Cell(2, 4).Value = "12345";
                    ws.Cell(5, 1).Value = "ИНСТРУКЦИЯ:"; ws.Cell(5, 1).Style.Font.Bold = true;
                    ws.Cell(6, 1).Value = "• Заполняйте со строки 2"; ws.Cell(7, 1).Value = "• Пол: 'Мужской'/'Женский'";
                    ws.Cell(8, 1).Value = "• Форма: 'Очная'/'Заочная'"; ws.Cell(9, 1).Value = "• Код: необязательно";
                    ws.Columns().AdjustToContents();
                    wb.SaveAs(saveDialog.FileName);
                }
                MessageBox.Show($"Шаблон сохранён:\n{saveDialog.FileName}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        // Навигация
        private void OpenSpiski_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Spiski_BPR.Spiski_BPR());
        private void OpenRaspisanie_BPR(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Raspisanie_BPR.Raspisanie_BPR());
        private void OpenClosed_BRP(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Closed_BRP.Closed_BRP());
        private void OpenGroups(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Groups.Groups());
        private void OpenDepartments(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
        private void OpenAuthorization(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.Authorization());
        private void OpenLessons(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Lessons.Lessons());
        private void OpenKabinets(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Kabinets.Kabinets());
        private void OpenExport(object sender, RoutedEventArgs e) => MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Export.Export());

        private void OpenNotificationSettings(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Classes.CurrentUser.Email))
            {
                System.Windows.MessageBox.Show("У вас не указан Email. Уведомления недоступны.", "Внимание", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }
            MainWindow.init.frame.Navigate(new Pages.NotificationSettings(Classes.CurrentUser.Email));
        }
    }
}