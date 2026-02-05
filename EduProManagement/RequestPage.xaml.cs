using EduProManagement.Data;
using EduProManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        private ObservableCollection<Request> _requests;
        private EduProDbContext _context;
        public RequestPage(Models.User user)
        {
            _context = new();
            _requests = new();
            bool isAdmin = user.Role.Name == "Администратор"; 
            InitializeComponent();
            if (!isAdmin)
            {
                UserBox.IsReadOnly = true;
                CourseBox.IsReadOnly = true;
                DateField.IsReadOnly = true;
            }
            List<string> datesort = new List<string>
            {
                "Все",
                "По убыванию",
                "По ворастанию"
            };
            DateSortBox.ItemsSource = datesort;
            List<string> statuses = _context.RequestStatuses.Select(r => r.Name).ToList();
            statuses.Add("Все");
            StatusSortBox.ItemsSource = statuses;
            LoadReuests();
            LoadComboBoxData();
        }

        private void LoadReuests()
        {
            _requests.Clear();
            var query = _context.Requests
                .Include(c => c.Course)
                .Include(c => c.Status)
                .Include(c => c.User).AsQueryable();
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchvalue = SearchBox.Text.ToLower();
                query = query.Where(q => q.Course.Name.ToLower().Contains(searchvalue) || q.Id.ToString().Contains(searchvalue));
            }
            if (DateSortBox.SelectedItem != null && DateSortBox.SelectedItem != "Все")
            {
                if (DateSortBox.SelectedItem == "По ворастанию")
                {
                    query = query.OrderBy(q => q.Date);
                }
                else
                {
                    query = query.OrderByDescending(q => q.Date);
                }
            }
            if (StatusSortBox.SelectedItem != null && StatusSortBox.SelectedItem != "Все")
            {
                query = query.Where(q => q.Status.Name == StatusSortBox.SelectedValue.ToString());
            }
            foreach (var req in query)
            {
                _requests.Add(req);
            }
            if (RequestsList.ItemsSource == null)
            {
                RequestsList.ItemsSource = _requests;
            }
        }
        private void LoadComboBoxData()
        {
            if (UserBox.ItemsSource == null)
            {
                UserBox.ItemsSource = _context.Users.ToList();
                UserBox.DisplayMemberPath = "FullName";
                UserBox.SelectedItemBinding = new Binding("User");
            }
            if (CourseBox.ItemsSource == null)
            {
                CourseBox.ItemsSource = _context.Courses.ToList();
                CourseBox.DisplayMemberPath = "Name";
                CourseBox.SelectedItemBinding = new Binding("Course");
            }
            if (StatusBox.ItemsSource == null)
            {
                StatusBox.ItemsSource = _context.RequestStatuses.ToList();
                StatusBox.DisplayMemberPath = "Name";
                StatusBox.SelectedItemBinding = new Binding("Status");
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadReuests();
        }

        private void DateSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadReuests();
        }

        private void StatusSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadReuests();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show($"Изменения сохранены","Успех",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении{ex.Message}"); 
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newrequest = new Request
            {
                Id = _requests.Count() + 1,
                Course = _context.Courses.FirstOrDefault(),
                User = _context.Users.FirstOrDefault(),
                Status = _context.RequestStatuses.First(s => s.Name == "Новая"),
                Date = DateOnly.MaxValue,

            };
            _requests.Add(newrequest);
            _context.Requests.Add(newrequest);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is not Request selectedrequest)
                return;

            var result = MessageBox.Show(
                $"Удалить заявку на курс{selectedrequest.Course.Name}?",
                "Подтверждение",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            _requests.Remove(selectedrequest);
            _context.Requests.Remove(selectedrequest);
        }
    }
}
