using BPRapp.Classes;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace BPRapp.Pages.MainMenuAdmins.ExportImport
{
    public partial class ExportImport : Page
    {
        public ExportImport()
        {
            InitializeComponent();
        }

        private void ExportToExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                var teachers = Users.Select().Where(u => u.Role == "Преподаватель").ToList();
                if (!teachers.Any())
                {
                    MessageBox.Show("Нет преподавателей для экспорта", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"Преподаватели_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Экспорт преподавателей в Excel"
                };

                if (saveDialog.ShowDialog() != true) return;

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Преподаватели");
                    var headers = new[] { "№", "ФИО", "Логин", "Пароль", "Роль", "Email", "Телефон" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = ws.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E8B57");
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    int row = 2;
                    foreach (var t in teachers.OrderBy(u => u.FIO))
                    {
                        ws.Cell(row, 1).Value = row - 1;
                        ws.Cell(row, 2).Value = t.FIO ?? "";
                        ws.Cell(row, 3).Value = t.Login ?? "";
                        ws.Cell(row, 4).Value = t.Parol ?? "";
                        ws.Cell(row, 5).Value = t.Role ?? "";
                        ws.Cell(row, 6).Value = t.Email ?? "";
                        ws.Cell(row, 7).Value = t.Phone_Number ?? "";
                        row++;
                    }
                    ws.Columns().AdjustToContents();
                    ws.Columns(1, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    workbook.SaveAs(saveDialog.FileName);
                }
                MessageBox.Show($"✅ Экспортировано {teachers.Count} преподавателей", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportFromExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    Title = "Выберите файл для импорта"
                };

                if (openFileDialog.ShowDialog() != true) return;

                int imported = 0, skipped = 0, errors = 0;

                using (var workbook = new XLWorkbook(openFileDialog.FileName))
                {
                    foreach (var ws in workbook.Worksheets)
                    {
                        int row = 2;
                        while (!ws.Cell(row, 1).IsEmpty())
                        {
                            try
                            {
                                var fio = ws.Cell(row, 2).GetValue<string>();
                                var login = ws.Cell(row, 3).GetValue<string>();
                                var password = ws.Cell(row, 4).GetValue<string>();
                                var email = ws.Cell(row, 6).GetValue<string>();
                                var phone = ws.Cell(row, 7).GetValue<string>();

                                if (string.IsNullOrWhiteSpace(fio) || string.IsNullOrWhiteSpace(login))
                                {
                                    row++;
                                    continue;
                                }

                                var existing = Users.Select()
                                    .FirstOrDefault(u => u.Login == login || u.FIO == fio);

                                if (existing != null)
                                {
                                    skipped++;
                                    row++;
                                    continue;
                                }

                                new Users(0, fio, login, password, "Преподаватель", email, phone).Add();
                                imported++;
                            }
                            catch
                            {
                                errors++;
                            }
                            row++;
                        }
                    }
                }

                importStatusTB.Text = $"✅ Добавлено: {imported} | ⏭ Пропущено: {skipped} | ❌ Ошибки: {errors}";

                MessageBox.Show($"Импорт завершён!\n✅ Добавлено: {imported}\n⏭ Пропущено (дубликаты): {skipped}\n❌ Ошибки: {errors}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка импорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                importStatusTB.Text = $"❌ Ошибка: {ex.Message}";
            }
        }

        private void DownloadTemplate(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = "Шаблон_Преподаватели_для_импорта.xlsx",
                    Title = "Сохранить шаблон"
                };

                if (saveDialog.ShowDialog() != true) return;

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Преподаватели");

                    var headers = new[] { "№", "ФИО*", "Логин*", "Пароль*", "Роль", "Email", "Телефон" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = ws.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E8B57");
                        cell.Style.Font.FontColor = XLColor.White;
                    }

                    ws.Cell(2, 1).Value = "1";
                    ws.Cell(2, 2).Value = "Иванов Иван Иванович";
                    ws.Cell(2, 3).Value = "ivanov";
                    ws.Cell(2, 4).Value = "SecurePass123";
                    ws.Cell(2, 5).Value = "Преподаватель";
                    ws.Cell(2, 6).Value = "ivanov@school.ru";
                    ws.Cell(2, 7).Value = "89001234567";

                    ws.Cell(4, 1).Value = "📋 ИНСТРУКЦИЯ:";
                    ws.Cell(4, 1).Style.Font.Bold = true;
                    ws.Cell(5, 1).Value = "• Заполняйте начиная со строки 2";
                    ws.Cell(6, 1).Value = "• Поля с * обязательны для заполнения";
                    ws.Cell(7, 1).Value = "• Роль: 'Преподаватель' или 'Администратор'";
                    ws.Cell(8, 1).Value = "• Не удаляйте заголовки столбцов";
                    ws.Cell(9, 1).Value = "• Сохраните файл перед импортом";

                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(saveDialog.FileName);
                }

                MessageBox.Show("Шаблон сохранён!\nЗаполните его и используйте для импорта.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportTo1C(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "XML файлы|*.xml",
                FileName = $"teachers_export_{DateTime.Now:yyyyMMdd}.xml",
                Title = "Экспорт преподавателей в 1C"
            };
            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    var teachers = Users.Select().Where(u => u.Role == "Преподаватель").ToList();

                    var doc = new XDocument(
                        new XElement("Teachers",
                            teachers.Select(t => new XElement("Teacher",
                                new XElement("FIO", t.FIO ?? ""),
                                new XElement("Login", t.Login ?? ""),
                                new XElement("Email", t.Email ?? ""),
                                new XElement("Phone", t.Phone_Number ?? ""),
                                new XElement("ExportDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                            ))
                        )
                    );
                    doc.Save(saveDialog.FileName);
                    MessageBox.Show($"Экспортировано {teachers.Count} преподавателей", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportFrom1C(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "XML файлы|*.xml",
                Title = "Импорт преподавателей из 1C"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var doc = XDocument.Load(openFileDialog.FileName);
                    var imported = 0;
                    var skipped = 0;
                    foreach (var elem in doc.Root?.Elements("Teacher") ?? Enumerable.Empty<XElement>())
                    {
                        var fio = elem.Element("FIO")?.Value;
                        var login = elem.Element("Login")?.Value;
                        var existing = Users.Select().FirstOrDefault(u => u.Login == login || u.FIO == fio);
                        if (existing != null)
                        {
                            skipped++;
                            continue;
                        }
                        var newUser = new Users(0, fio, login, "temp123", "Преподаватель", elem.Element("Email")?.Value, elem.Element("Phone")?.Value);
                        newUser.Add();
                        imported++;
                    }
                    MessageBox.Show($"Импорт завершён!\n✅ Добавлено: {imported}\n⏭ Пропущено (дубликаты): {skipped}", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenAdmins_List(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Admins_List.Admins_List());

        private void OpenTeachers_List(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.Teachers_List.Teachers_List());

        private void OpenAuthorization(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.Authorization());

        private void OpenExportImport(object sender, RoutedEventArgs e) =>
            MainWindow.init.frame.Navigate(new Pages.MainMenuAdmins.ExportImport.ExportImport());
    }
}