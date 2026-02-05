using EduProManagement.Data;
using EduProManagement.Models;
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
using System.Windows.Shapes;

namespace EduProManagement
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private EduProDbContext _context;
        public LoginWindow()
        {
            InitializeComponent();
            _context = new();
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == LoginBox.Text && u.Password == PasswordBox.Password);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainWindow window = new MainWindow(user);
            window.Show();
            this.Close();
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            var user = new User()
            {
                Role = new Role()
                {
                    Name = null
                }
            };
            MainWindow window = new MainWindow(user);
            window.Show();
            this.Close();
        }
    }
}
