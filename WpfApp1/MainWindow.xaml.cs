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

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
   
    }

    private void Button_Click_Task1(object sender, RoutedEventArgs e)
    {

    }

    private void Button_Click_Task2(object sender, RoutedEventArgs e)
    {
        Task2 task = new Task2();
        task.Show();
        this.Close();
    }

    private void Button_Click_Task3(object sender, RoutedEventArgs e)
    {
        Task3 task = new Task3();
        task.Show();
        this.Close();
    }
}