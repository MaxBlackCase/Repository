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

namespace Command
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      CommandBinding comBind = new CommandBinding();
      comBind.Command = ApplicationCommands.Help;
      comBind.Executed += CommandBinding_Executed;
      helpButton.CommandBindings.Add(comBind);

    }

    private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      MessageBox.Show("Справка по приложению");
    }
  }
}
