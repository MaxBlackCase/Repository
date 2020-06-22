using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_application
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      ListBox nList = new ListBox();
      nList.Items.Add("One");
      nList.Items.Add("Two");
      nList.Items.Add("Three");

      products.Items.Add(new TabItem
      {
        Header = new TextBlock { Text = "tab_Three" },
        Content = nList
      });

    }

    
  }
 

}