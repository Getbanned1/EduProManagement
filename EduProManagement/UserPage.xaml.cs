using Azure.Core;
using EduProManagement.Data;
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
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private ObservableCollection<Request> _users;
        private EduProDbContext _context;
        public UserPage()
        {
            _context = new();
            _users = new();
            InitializeComponent();
            LoadReuests();
        }
        private void LoadReuests()
        {
            _users.Clear();
            var query = _context.Requests
                .Include(c => c.Course)
                .Include(c => c.Status)
                .Include(c => c.User).AsQueryable();
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchvalue = SearchBox.Text.ToLower();
                query = query.Where(q => q.User.FullName.ToLower().Contains(searchvalue) || q.Id.ToString().Contains(searchvalue));
            }
            foreach (var req in query)
            {
                _users.Add(req);
            }
            if (RequestsList.ItemsSource == null)
            {
                RequestsList.ItemsSource = _users;
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
