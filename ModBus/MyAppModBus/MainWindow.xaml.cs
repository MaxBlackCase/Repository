using InteractiveDataDisplay.WPF;
using MahApps.Metro.Controls;
using Modbus.Device;
using MyAppModBus.Controllers;
using MyAppModBus.ViewModel;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyAppModBus
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {

    const byte slaveID = 1;

    private readonly ushort startAddress = 0;
    private readonly ushort numburOfPoints = 18;
    private string _text;
    private double _countTime;

    public static string result;
    private DispatcherTimer timer;
    private static SerialPort _serialPort = null;
    private static ModbusSerialMaster _master = null;

    private LineGraph[][] _seriesArr = new LineGraph[2][];
    private string[][] nameLineSeries = new string[2][];
    private int _readWriteTimeOut = 20;

    private UIElement[][] uiElements = new UIElement[2][];

    private SfChartViewModel vmSfCh = new SfChartViewModel();

    #region Словари для данныч линий 
    private Dictionary<double, double> volltage = new Dictionary<double, double>();
    private Dictionary<double, double> current = new Dictionary<double, double>();
    private Dictionary<double, double> torque = new Dictionary<double, double>();
    private Dictionary<double, double> tempExternal = new Dictionary<double, double>();
    private Dictionary<double, double> tempMotor = new Dictionary<double, double>();


    private Dictionary<double, double>[][] _arrDict = new Dictionary<double, double>[2][];
    #endregion

    /// <summary>
    /// Главноe окно
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();
    }

    //Инициализация портов
    //internal void AddItemToComboBox()
    //{
    //  //Получение портов
    //  string[] ports = SerialPort.GetPortNames();
    //  foreach (string port in ports)
    //  {
    //    if (port == "")
    //    {
    //      comboBoxMainPorts.Items.Add("Отсутствует порт");
    //    }
    //    else
    //    {
    //      string str = port.ToString();
    //      int maxLength = 3;
    //      string result = str.Substring(0, Math.Min(str.Length, maxLength));

    //      if (result == "COM")
    //      {
    //        comboBoxMainPorts.Items.Add(port);
    //      }
    //    }
    //  }
    //  comboBoxMainPorts.SelectedIndex = 0;
    //}

    /// <summary>
    /// Подключение по SerilaPort(RTU) к Master устройству
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConnectToDevice(object sender, RoutedEventArgs e)
    {
      _serialPort = new SerialPort();
      timer = new DispatcherTimer();
      try
      {
        if (_serialPort.IsOpen)
        {
          _serialPort.Close();
          disconnectComPort.Visibility = Visibility.Hidden;

        }

        #region <Настройки RTU подключения>
        _serialPort.PortName = "COM27";
        _serialPort.BaudRate = 119200;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = _readWriteTimeOut;
        _serialPort.WriteTimeout = _readWriteTimeOut;
        _serialPort.DtrEnable = true;
        _serialPort.Open();
        #endregion

        _master = ModbusSerialMaster.CreateRtu(_serialPort);
        //Сброс регистров
        ResetRegisters();

        uiElements[0] = new UIElement[] { checkBoxWrite_1, checkBoxWrite_2, checkBoxWrite_3, StartRegsRequest };
        uiElements[1] = new UIElement[] { decTextBox, decButtonTimeout, comboBoxMainPorts };


        for (int i = 0; i < 1; i++)
        {
          foreach (var tr in uiElements[0])
          {
            tr.IsEnabled = true;
          }
          foreach (var fls in uiElements[1])
          {
            fls.IsEnabled = false;
          }
        }

        connectComPort.Visibility = Visibility.Hidden;
        disconnectComPort.Visibility = Visibility.Visible;
        textViewer.Text = $"Порт {_serialPort.PortName} Подключен";

      }
      catch (Exception err)
      {
        textViewer.Text = $"Ошибка: {err.Message}";
      }


    }

    /// <summary>
    /// Отключение устройства
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisconnectToDevice(object sender, RoutedEventArgs e)
    {
      timer.Stop();
      _serialPort.Close();
      _serialPort.Dispose();

      for (int i = 0; i < 1; i++)
      {
        foreach (var tr in uiElements[0])
        {
          tr.IsEnabled = false;
        }
        foreach (var fls in uiElements[1])
        {
          fls.IsEnabled = true;
        }
        foreach (UIElement toggle in uiElements[0])
        {
          toggle.IsEnabled = false;
        }
      }

      disconnectComPort.Visibility = Visibility.Hidden;
      connectComPort.Visibility = Visibility.Visible;
      textViewer.Text = $"Порт {_serialPort.PortName} закрыт";
      StartRegsRequest.Content = "Запустить";
    }

    /// <summary>
    /// Получение данных с регистров
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    internal double countTime = 0;
    internal double countIndex = 0;
    private int[][] _numberRegisters = new int[2][];

    private DateTime _tSpan;

    private void GetHoldReg(object sender, EventArgs e)
    {
      ushort[] result = _master.ReadHoldingRegisters(slaveID, startAddress, numburOfPoints);

      try
      {

        textViewer.Text = null;
        countTime += _readWriteTimeOut;
        _tSpan = DateTime.Now;


        ///Вывод всех регистров на экран
        for (int i = 0; i < result.Length; i++)
        {
          textViewer.Text += $"Регистр: {i} \t{result[i]}\n";

        }

        ///Запуск функции отображения концевиков
        SetValSingleRegister(result[9], result[10]);


        ///Занесение значений на график
        if (countTime % _readWriteTimeOut == 0)
        {
          #region OldGraphs
          //for ( int valueFirstChart = 0; valueFirstChart < _arrDict[ 0 ].Count(); valueFirstChart++ ) {
          //  _arrDict[ 0 ][ valueFirstChart ].Add( countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 0 ][ valueFirstChart ] ] ) );
          //  //Очищение коллекции точек График 1
          //  if (_arrDict[0][valueFirstChart].Count > 1000) { _arrDict[0][valueFirstChart].Clear(); }
          //}
          //for ( int valueSecondChart = 0; valueSecondChart < _arrDict[ 1 ].Count(); valueSecondChart++ ) {
          //  _arrDict[ 1 ][ valueSecondChart ].Add(countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 1 ][ valueSecondChart ] ] ) );
          //  //Очищение коллекции точек График 2
          //  if (_arrDict[1][valueSecondChart].Count > 1000){_arrDict[1][valueSecondChart].Clear(); }
          //}

          //for ( int valueFirstChart = 0; valueFirstChart < _linesArr[ 0 ].Length; valueFirstChart++ ) {
          //  _linesArr[ 0 ][ valueFirstChart ].Plot( _arrDict[ 0 ][ valueFirstChart ].Keys, _arrDict[ 0 ][ valueFirstChart ].Values );
          //}
          //for ( int valueSecondChart = 0; valueSecondChart < _linesArr[ 1 ].Length; valueSecondChart++ ) {
          //  _linesArr[ 1 ][ valueSecondChart ].Plot( _arrDict[ 1 ][ valueSecondChart ].Keys, _arrDict[ 1 ][ valueSecondChart ].Values );
          //}
          #endregion

          #region NewGraphs
          #endregion

        }

        countIndex++;
      }
      catch (Exception err)
      {
        textViewer.Text = $"Ошибка: {err.Message}";
      }


      #region Просчет контрольной суммы, если понадобится
      //Создание запроса

      //byte[] frame = new byte[ 8 ];
      //frame[ 0 ] = slaveID;
      //frame[ 1 ] = funcCode;
      //frame[ 2 ] = (byte)(startAddr >> 8);
      //frame[ 3 ] = (byte)startAddr;
      //frame[ 4 ] = (byte)(numOfPoints >> 8);
      //frame[ 5 ] = (byte)numOfPoints;
      //byte[] checkSum = CRC16( frame );
      //frame[ 6 ] = checkSum[ 0 ];
      //frame[ 7 ] = checkSum[ 1 ];

      ////Вывод в тектовое окно
      //textViewer.Text += "\n\n";
      //foreach ( var item in frame ) {

      //  textViewer.Text += string.Format( "{0:X2} ", item );

      //}

      /// <summary>
      /// Метод подсчет контрольной суммы
      /// </summary>
      /// <param name = "data" > Данные массива запроса</param>
      /// <returns></returns>
      //private static byte[] CRC16( byte[] data ) {

      //  byte[] checkSum = new byte[ 2 ];
      //  ushort reg_crc = 0xFFFF;
      //  for ( int i = 0; i < data.Length; i++ ) {
      //    if ( (reg_crc & 0x01) == 1 ) {
      //      reg_crc = (ushort)((reg_crc >> 1) ^ 0xA001);
      //    }
      //    else {
      //      reg_crc = (ushort)(reg_crc >> 1);
      //    }
      //  }

      //  checkSum[ 1 ] = (byte)((reg_crc >> 8) & 0xFF);
      //  checkSum[ 0 ] = (byte)(reg_crc & 0xFF);
      //  return checkSum;
      //}
      #endregion

    }

    /// <summary>
    /// Получение данных концевиков
    /// </summary>
    private void SetValSingleRegister(ushort registrNine, ushort registrTen)
    {

      try
      {
        if (_serialPort.IsOpen)
        {

          int[] arrLimitSwitch = new int[2];
          arrLimitSwitch[0] = Convert.ToInt32(registrNine);
          arrLimitSwitch[1] = Convert.ToInt32(registrTen);


          for (int i = 0; i < LimSwPanel.Children.Count; i++)
          {
            LimSwPanel.Children.Clear();
          }

          foreach (var item in arrLimitSwitch)
          {
            if (item == 1)
            {
              Ellipse LimSwEllipse = new Ellipse
              {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green,
                Margin = new Thickness(0, 0, 10, 15)
              };
              LimSwPanel.Children.Add(LimSwEllipse);
            }
            else
            {
              Ellipse LimSwEllipse = new Ellipse
              {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                Margin = new Thickness(0, 0, 10, 15)
              };
              LimSwPanel.Children.Add(LimSwEllipse);
            }
          }

        }

      }
      catch (Exception err)
      {

        textViewer.Text = $"Ошибка: {err.Message}";
      }

    }

    /// <summary>
    /// Ввод целочисленного значения в тектовое поле
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBoxDecimalPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }
    /// <summary>
    /// Задает время опроса устройства в ms
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    public int ReadWriteTimeOut
    {
      get => _readWriteTimeOut;
      set => _readWriteTimeOut = value;
    }

    private void DecimalButtonTimeoutClic(object sender, RoutedEventArgs e)
    {
      if (decTextBox.Text != "")
      {
        double valTextBox = Convert.ToDouble(decTextBox.Text);

        if (valTextBox < 20)
        {
          ReadWriteTimeOut = 20;
          textViewer.Text = $"Интервал не может быть меньше {_readWriteTimeOut} ms, поэтому задан интервал по умолчанию {_readWriteTimeOut} ms.";
        }
        else if (valTextBox > 100)
        {
          ReadWriteTimeOut = 100;
          textViewer.Text = $"Значение не может превышать значение в {_readWriteTimeOut} ms, поэтому задано значение по умолчанию {_readWriteTimeOut} ms.";
        }
        else
        {
          ReadWriteTimeOut = (int)valTextBox;
          textViewer.Text = $"Значение интервала опроса устроства: {_readWriteTimeOut} ms";
        }
      }

    }

    /// <summary>
    /// Сброс регистров 
    /// </summary>
    private void ResetRegisters()
    {
      ushort[] arrRegisters = new ushort[] { 6, 7, 8 };

      for (int i = 0; i < arrRegisters.Length; i++)
      {
        _master.WriteSingleRegister(slaveID, arrRegisters[i], 0);
      }
    }

    /// <summary>
    /// Отрисовка графиков и их линий
    /// </summary>

    //private void GraphLines( double thickness ) {

    //  var rand = new Random();

    //  _numberRegisters[ 0 ] = new int[ 3 ] { 0, 1, 4 };
    //  _numberRegisters[ 1 ] = new int[ 2 ] { 2, 3 };

    //  _linesArr[ 0 ] = new LineGraph[ 3 ];
    //  _linesArr[ 1 ] = new LineGraph[ 2 ];

    //  _arrDict[ 0 ] = new Dictionary<double, double>[ 3 ] { volltage, current, torque };
    //  _arrDict[ 1 ] = new Dictionary<double, double>[ 2 ] { tempExternal, tempMotor };

    //  nameLines[ 0 ] = new string[] { "Volltage", "Current", "Torque" };
    //  nameLines[ 1 ] = new string[] { "External", "Motor" };


    //  // Линии первого графика
    //  for ( int linesFirstChart = 0; linesFirstChart < _arrDict[ 0 ].Length; linesFirstChart++ ) {
    //    var lines = new LineGraph
    //    {

    //      Description = String.Format( $"{nameLines[ 0 ][ linesFirstChart ]}" ),
    //      StrokeThickness = thickness,
    //      Stroke = new SolidColorBrush(Color.FromRgb( (byte)rand.Next( 1, 255 ), (byte)rand.Next( 1, 255 ), (byte)rand.Next(1, 255)))

    //    };

    //    lines_one.Children.Add( lines );
    //    _linesArr[ 0 ][ linesFirstChart ] = lines;

    //  }

    //  //Линии второго графика
    //  for ( int linesSecondChart = 0; linesSecondChart < _arrDict[ 1 ].Length; linesSecondChart++ ) {
    //    var lines = new LineGraph
    //    {
    //      Description = String.Format( $"{nameLines[ 1 ][ linesSecondChart ]}" ),
    //      StrokeThickness = thickness,
    //      Stroke = new SolidColorBrush( Color.FromRgb( (byte)rand.Next( 1, 255 ), (byte)rand.Next( 1, 255 ), (byte)rand.Next( 1, 255 ) ) )
    //    };

    //    lines_two.Children.Add( lines );
    //    _linesArr[ 1 ][ linesSecondChart ] = lines;
    //  }
    //}

    /// <summary>
    /// Запрос из регистров Master(a) и запусе/остановка таймера
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RegistersRequest(object sender, RoutedEventArgs e)
    {
      var BtnStartTimerAndRegistersRequest = StartRegsRequest;
      try
      {
        if (_serialPort.IsOpen && timer.IsEnabled == false)
        {
          #region <Timer>
          timer.Tick += new EventHandler(GetHoldReg);
          timer.Interval = new TimeSpan(0, 0, 0, 0, _readWriteTimeOut);
          timer.Start();
          #endregion
          BtnStartTimerAndRegistersRequest.Content = "Остановить";
        }
        else
        {
          timer.Stop();
          BtnStartTimerAndRegistersRequest.Content = "Запустить";
        }
      }
      catch (Exception err)
      {
        textViewer.Text = $"Ошибка: {err.Message}";
      }
    }

    /// <summary>
    /// Запись в регистры
    /// </summary>
    /// <param name="sender">Объект</param>
    /// <param name="e">Событие</param>
    private void CheсkValToRegisters(object sender, RoutedEventArgs e)
    {
      ToggleSwitch toggleSwitch = sender as ToggleSwitch;
      var indElem = CheckBoxWriteRegisters.Children.IndexOf(toggleSwitch);
      ushort[] arrRegisters = new ushort[] { 6, 7, 8 };
      if (toggleSwitch != null && _serialPort.IsOpen)
      {
        if (toggleSwitch.IsOn == true)
        {
          for (var i = 0; i < arrRegisters.Length; i++)
          {
            if (i == indElem)
            {
              _master.WriteSingleRegister(slaveID, arrRegisters[i], 1);
            }
          }
        }
        else
        {
          for (var i = 0; i < arrRegisters.Length; i++)
          {
            if (i == indElem)
            {
              _master.WriteSingleRegister(slaveID, arrRegisters[i], 0);
            }
          }
        }
      }
    }

    //private void GetSfCharts()
    //{

    //  Data = new List<DataPoints>() {

    //    new DataPoints {Name = "David", Height = 180},
    //    new DataPoints {Name = "Max", Height = 150},
    //    new DataPoints {Name = "Ulya", Height = 178},
    //    new DataPoints {Name = "Brad", Height = 162},

    //  };

    //  SfChart ch = new SfChart();

    //  CategoryAxis primaryAxis = new CategoryAxis();

    //  primaryAxis.Header = @"/Name/";

    //  ch.PrimaryAxis = primaryAxis;

    //  NumericalAxis seceondoryAxis = new NumericalAxis();
    //  seceondoryAxis.Header = "Height(in cm)";
    //  ch.SecondaryAxis = seceondoryAxis;

    //  ColumnSeries sers = new ColumnSeries();

    //  sers.ItemsSource = this.Data;
    //  sers.XBindingPath = "Name";
    //  sers.YBindingPath = "Height";

    //  ch.Series.Add(sers);
    //}

  }

  public class VisibilityToCheckedConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return ((Visibility)value) == Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
    }
  }

}
