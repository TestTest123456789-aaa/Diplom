using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BPRapp.Pages.MainMenuTeachers.Specialties
{
    public partial class Specialties : Page
    {
        private int? departmentId;
        private List<Classes.Specialties> _allSpecialties = new List<Classes.Specialties>();

        public Specialties(int deptId) : this()
        {
            departmentId = deptId;
        }

        public Specialties()
        {
            InitializeComponent();
            Loaded += Specialties_Loaded;
        }

        private void Specialties_Loaded(object sender, RoutedEventArgs e)
        {
            if (departmentId.HasValue)
            {
                var dept = Classes.Departments.Select().FirstOrDefault(d => d.Id == departmentId.Value);
                if (dept != null)
                {
                    deptNameLabel.Content = $"📂 Отделение: {dept.Name}";
                    _allSpecialties = Classes.Specialties.SelectByDepartment(departmentId.Value);
                    LoadSpecialties();
                }
            }
        }

        private void LoadSpecialties(string searchText = "")
        {
            SpecialtiesParent.Children.Clear();

            var filtered = string.IsNullOrWhiteSpace(searchText)
                ? _allSpecialties
                : _allSpecialties.Where(s => s.Name.ToLower().Contains(searchText.ToLower()) ||
                                            s.Code.Contains(searchText));

            foreach (var spec in filtered.OrderBy(s => s.Code))
            {
                SpecialtiesParent.Children.Add(new SpecialtyItem(spec));
            }
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadSpecialties(searchTB.Text);
        }

        private void AddSpecialty(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new AddSpecialty(deptId: departmentId));
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.MainMenuTeachers.Departments.Departments());
        }
    }
}