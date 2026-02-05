using EduProManagement.Data;
using Microsoft.EntityFrameworkCore;
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

namespace EduProManagement
{
    /// <summary>
    /// Логика взаимодействия для CoursePage.xaml
    /// </summary>
    public partial class CoursePage : Page
    {
        private EduProDbContext _context;
        public CoursePage(Models.User user)
        {
            InitializeComponent();
            _context = new();
            SearchPanel.Visibility = user.Role.Name != null ? Visibility.Visible : Visibility.Hidden;
            List<string> datesort = new List<string>
            {
                "Все",
                "По убыванию",
                "По ворастанию"
            };
            DateSortBox.ItemsSource = datesort;
            LoadCourses();
        }

        public void LoadCourses()
        {
            

            var query = _context.Courses
                .Include(c => c.CourseType)
                .Include(c => c.Requests)
                .Include(c => c.TeacherType).AsQueryable();
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchvalue = SearchBox.Text.ToLower();
                query = query.Where(q => q.Name.ToLower().Contains(searchvalue));
            }
            if (DateSortBox.SelectedItem != null && DateSortBox.SelectedItem != "Все")
            {
                if (DateSortBox.SelectedItem == "По ворастанию")
                {
                    query = query.OrderBy(q => q.StartDate);
                }
                else
                {
                    query = query.OrderByDescending(q => q.StartDate);
                }
            }
            CourseList.ItemsSource = query.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCourses();
        }

        private void DateSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCourses();
        }
    }
}
