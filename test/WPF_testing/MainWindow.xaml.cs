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


namespace WPF_testing
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void openModalWindow(object sender, RoutedEventArgs e)
    {
      win_two winTwo = new win_two("Modal_Window");
      winTwo.Show();
    }

    private void closeMainWindow(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}
