using EduProManagement.Data;
using EduProManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
            //if (!isAdmin)
            //{
            //    UserBox.IsReadOnly = true;
            //    CourseBox.IsReadOnly = true;
            //    DateField.IsReadOnly = true;
            //}
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

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newrequest = new Request
            {
                Id = _requests.Count() + 1,
                Course = _context.Courses.FirstOrDefault(),
                User = _context.Users.FirstOrDefault(),
                Status = _context.RequestStatuses.First(s => s.Name == "Новая"),
                Date = DateOnly.MaxValue,

            };
            await _context.Requests.AddAsync(newrequest);
            _context.SaveChanges();
            LoadReuests();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem is not Request selectedrequest)
                return;

            var result = MessageBox.Show(
                $"Удалить заявку на курс {selectedrequest.Course.Name}?",
                "Подтверждение",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;


            _context.Requests.Remove(selectedrequest);
            _context.SaveChanges();
            LoadReuests();
        }

        private void RequestsList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedRequest = e.Row.Item as Request;
            if (editedRequest == null) return;

            var column = e.Column.Header.ToString();

            if (column == "Статус" && e.EditingElement is ComboBox cb)
            {
                var newStatus = cb.SelectedItem as RequestStatus;
                if (newStatus != null)
                {
                    string oldStatusName = editedRequest.Status?.Name;

                    // Проверяем изменение статуса
                    if (oldStatusName != "Подтверждена" && newStatus.Name == "Подтверждена")
                    {
                        // ПОДТВЕРЖДАЕМ заявку - уменьшаем AvaliableSpace
                        if (!DecreaseAvailableSpace(editedRequest.CourseId))
                        {
                            // Если нет мест, отменяем изменение
                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (oldStatusName == "Подтверждена" && newStatus.Name != "Подтверждена")
                    {
                        // ОТМЕНЯЕМ подтверждение - увеличиваем AvaliableSpace
                        IncreaseAvailableSpace(editedRequest.CourseId);
                    }

                    // Обновляем статус
                    editedRequest.Status = newStatus;
                    editedRequest.StatusId = newStatus.Id;

                    // Обновляем SeatsTaken для заявок этого курса


                    _context.SaveChanges();
                }
            }
        }

        // Уменьшение свободных мест (при подтверждении)
        private bool DecreaseAvailableSpace(int courseId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) return false;

            if (course.AvaliableSpace > 0)
            {
                course.AvaliableSpace--;  // Уменьшаем на 1
                _context.SaveChanges();
                return true;
            }
            else
            {
                MessageBox.Show($"Нет свободных мест на курсе \"{course.Name}\"!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        // Увеличение свободных мест (при отмене подтверждения)
        private void IncreaseAvailableSpace(int courseId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) return;

            course.AvaliableSpace++;  // Увеличиваем на 1
            _context.SaveChanges();
        }
       
    }
}
