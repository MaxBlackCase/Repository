using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MyAppModBus {
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    /// <summary>
    /// Главноe окно
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }
      /// <summary>
      /// Ввод целочисленного значения в тектовое поле
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void CheckReadWriteOnRegular(object sender, TextCompositionEventArgs e)
      {
         e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
      }

   }

  //public class VisibilityToCheckedConverter : IValueConverter
  //{
  //  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  //  {
  //    return ((Visibility)value) == Visibility.Visible;
  //  }

  //  public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  //  {
  //    return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
  //  }
  //}

}
