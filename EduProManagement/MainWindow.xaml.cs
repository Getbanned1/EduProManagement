using EduProManagement.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EduProManagement;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private User _user;
    public MainWindow(Models.User user)
    {
        InitializeComponent();
        _user = user;
        NavigationPanel.Visibility = user.Role.Name == "Администратор" || user.Role.Name == "Менеджер по обучению" ? Visibility.Visible : Visibility.Hidden;

        MainFrame.Navigate(new CoursePage(_user));
    }

    private void CoursePage_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new CoursePage(_user));
    }

    private void RequestPage_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new RequestPage(_user));
    }
}