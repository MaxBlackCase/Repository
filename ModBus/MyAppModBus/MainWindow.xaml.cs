using Modbus.Device;
using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using LiveCharts;
using LiveCharts.Wpf;
namespace MyAppModBus {
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    const byte slaveID = 1;

    private readonly ushort startAddress = 0;
    private readonly ushort numburOfPoints = 18;
    private int readWriteTimeOut = 50;

    public static string result;
    private DispatcherTimer timer;
    public static SerialPort _serialPort = null;
    public static ModbusSerialMaster master = null;

    public ChartValues<int> voltageValues = new ChartValues<int>();
    public ChartValues<int> currentValues = new ChartValues<int>();
    public ChartValues<int> torqueValues = new ChartValues<int>();

    public SeriesCollection SeriesCollection { get; private set; }
    public int[] Labels { get; set; }
    public Func<int, string> YFormatter { get; set; }

    public MainWindow() {
      InitializeComponent();

      //ScheduleGet();
      AddItemToComboBox();
      btnGetHoldReg.IsEnabled = false;

    }

    //Инициализация портов
    private void AddItemToComboBox() {
      //Получение портов
      string[] ports = SerialPort.GetPortNames();
      foreach ( string port in ports ) {
        if ( port == "" ) {
          comboBoxMainPorts.Items.Add( "Отсутствует порт" );
        }
        else {
          string str = port.ToString();
          int maxLength = 3;
          string result = str.Substring( 0, Math.Min( str.Length, maxLength ) );

          if ( result == "COM" ) {
            comboBoxMainPorts.Items.Add( port );
          }
        }
      }
      comboBoxMainPorts.SelectedIndex = 0;
    }

    /// <summary>
    /// Подключение по SerilaPort(RTU) к Master устройству
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConnectToDevice( object sender, RoutedEventArgs e ) {
      _serialPort = new SerialPort();
      timer = new DispatcherTimer();

      try {
        if ( _serialPort.IsOpen ) {
          _serialPort.Close();
          disconnectComPort.Visibility = Visibility.Hidden;

        }

        #region <Настройки RTU подключения>
        _serialPort.PortName = comboBoxMainPorts.Text;
        _serialPort.BaudRate = 119200;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = readWriteTimeOut;
        _serialPort.WriteTimeout = readWriteTimeOut;
        _serialPort.DtrEnable = true;
        _serialPort.Open();
        #endregion

        master = ModbusSerialMaster.CreateRtu( _serialPort );
        #region <Timer>
        timer.Tick += new EventHandler( GetHoldReg );
        timer.Interval = new TimeSpan( 0, 0, 0, 0, readWriteTimeOut );
        timer.Start();
        #endregion

        //Сброс регистров
        ResetRegisters();

        checkBoxWrite_1.IsEnabled = true;
        checkBoxWrite_2.IsEnabled = true;
        checkBoxWrite_3.IsEnabled = true;
        btnGetHoldReg.IsEnabled = true;
        decTextBox.IsEnabled = false;
        decButtonTimeout.IsEnabled = false;
        comboBoxMainPorts.IsEnabled = false;
        disconnectComPort.Visibility = Visibility.Visible;
        textViewer.Text = $"Порт {_serialPort.PortName} Подключен";

      }
      catch ( Exception err ) {
        _serialPort.Close();
        connectComPort.Content = "Подключить";
        comboBoxMainPorts.IsEnabled = true;
        decButtonTimeout.IsEnabled = false;
        disconnectComPort.Visibility = Visibility.Hidden;
        textViewer.Text = $"Ошибка: {err.Message}";
      }
    }

    /// <summary>
    /// Отключение устройства
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisconnectToDevice( object sender, RoutedEventArgs e ) {
      timer.Stop();
      _serialPort.Close();
      comboBoxMainPorts.IsEnabled = true;
      disconnectComPort.Visibility = Visibility.Hidden;
      textViewer.Text = $"Порт {_serialPort.PortName} закрыт";
      decButtonTimeout.IsEnabled = true;
      decTextBox.IsEnabled = true;
      btnGetHoldReg.IsEnabled = false;
      btnGetHoldReg.IsEnabled = false;
      #region checkBoxWrite
      checkBoxWrite_1.IsChecked = false;
      checkBoxWrite_2.IsChecked = false;
      checkBoxWrite_3.IsChecked = false;
      checkBoxWrite_1.IsEnabled = false;
      checkBoxWrite_2.IsEnabled = false;
      checkBoxWrite_3.IsEnabled = false;
      #endregion
    }


    /// <summary>
    /// Получение данных с регистров
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private int timeCount = 0;

    private void GetHoldReg( object sender, EventArgs e ) {

      ushort[] result = master.ReadHoldingRegisters( slaveID, startAddress, numburOfPoints );
      try {
        textViewer.Text = "";
        int i = 0;
        timeCount += readWriteTimeOut;
        foreach ( ushort item in result ) {
          textViewer.Text += $"Регистр: {i} \t{item}\n";
          i++;
        }

        SetValSingleRegister( result[ 9 ], result[ 10 ] );
        if ( timeCount % 1000 == 0 ) {
          voltageValues.Add( result[ 0 ] );
          currentValues.Add( result[ 1 ] );
          torqueValues.Add( result[ 4 ] );
          ScheduleGet();
        }

      }
      catch ( Exception err ) {

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
    private void SetValSingleRegister( ushort registrNine, ushort registrTen ) {

      try {
        if ( _serialPort.IsOpen ) {

          int[] arrLimitSwitch = new int[ 2 ];
          arrLimitSwitch[ 0 ] = Convert.ToInt32( registrNine );
          arrLimitSwitch[ 1 ] = Convert.ToInt32( registrTen );


          for ( int i = 0; i < LimSwPanel.Children.Count; i++ ) {
            LimSwPanel.Children.Clear();
          }

          foreach ( var item in arrLimitSwitch ) {
            if ( item == 1 ) {
              Ellipse LimSwEllipse = new Ellipse
              {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green,
                Margin = new Thickness( 10, 5, 10, 5 )
              };
              LimSwPanel.Children.Add( LimSwEllipse );
            }
            else {
              Ellipse LimSwEllipse = new Ellipse
              {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                Margin = new Thickness( 10, 5, 10, 5 )
              };
              LimSwPanel.Children.Add( LimSwEllipse );
            }
          }

        }

      }
      catch ( Exception err ) {

        textViewer.Text = $"Ошибка: {err.Message}";
      }

    }


    /// <summary>
    /// Проверка записаных данных в регистры
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckValToRegisters( object sender, RoutedEventArgs e ) {
      try {
        ToggleButton pressed = (ToggleButton)sender;
        var indElem = CheckBoxWriteRegisters.Children.IndexOf( pressed );
        ushort[] arrRegisters = new ushort[] { 6, 7, 8 };

        if ( _serialPort.IsOpen && Convert.ToBoolean( pressed.IsChecked ) == true ) {
          for ( var i = 0; i < arrRegisters.Length; i++ ) {
            if ( i == indElem ) {
              master.WriteSingleRegister( slaveID, arrRegisters[ i ], 1 );
            }
          }
        }
      }
      catch ( Exception err ) {
        textViewer.Text = $"Ошибка: {err.Message}";
      }
    }

    /// <summary>
    /// Снятие ограничений в регистрах
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UncheckValToRegisters( object sender, RoutedEventArgs e ) {
      try {
        ToggleButton pressed = (ToggleButton)sender;
        var indElem = CheckBoxWriteRegisters.Children.IndexOf( pressed );
        ushort[] arrRegisters = new ushort[] { 6, 7, 8 };

        if ( _serialPort.IsOpen && Convert.ToBoolean( pressed.IsChecked ) == false ) {
          for ( var i = 0; i < arrRegisters.Length; i++ ) {
            if ( i == indElem ) {
              master.WriteSingleRegister( slaveID, arrRegisters[ i ], 0 );
            }
          }
        }
      }
      catch ( Exception err ) {
        textViewer.Text = $"Ошибка: {err.Message}";
      }
    }


    /// <summary>
    /// Ввод целочисленного значения в тектовое поле
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBoxDecimalPreviewTextInput( object sender, TextCompositionEventArgs e ) {
      e.Handled = new Regex( "[^0-9]+" ).IsMatch( e.Text );
    }
    /// <summary>
    /// Задает время опроса устройства в ms
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DecimalButtonTimeoutClic( object sender, RoutedEventArgs e ) {
      if ( decTextBox.Text != "" ) {
        double valTextBox = Convert.ToDouble( decTextBox.Text );

        if ( valTextBox < 50 ) {
          readWriteTimeOut = 50;
          textViewer.Text = $"Интервал не может быть меньше {readWriteTimeOut} ms, поэтому задан интервал по умолчанию {readWriteTimeOut} ms.";
        }
        else if ( valTextBox > 1000 ) {
          readWriteTimeOut = 1000;
          textViewer.Text = $"Значение не может превышать значение в {readWriteTimeOut} ms, поэтому задано значение по умолчанию {readWriteTimeOut} ms.";
        }
        else {
          readWriteTimeOut = (int)valTextBox;
          textViewer.Text = $"Значение интервала опроса устроства: {readWriteTimeOut} ms";
        }
      }

    }
    /// <summary>
    /// Сброс регистров 
    /// </summary>
    private void ResetRegisters() {
      ushort[] arrRegisters = new ushort[] { 6, 7, 8 };

      for ( int i = 0; i < arrRegisters.Length; i++ ) {
        master.WriteSingleRegister( slaveID, arrRegisters[ i ], 0 );
      }
    }

    public void ScheduleGet() {

      SeriesCollection = new SeriesCollection
      {
        new LineSeries
        {
            Title = "Voltage",
            Values = voltageValues,
            PointGeometry = null
        },
        new LineSeries
        {
            Title = "Current",
            Values = currentValues,
            PointGeometry = null
        },
        new LineSeries
        {
            Title = "Torque",
            Values = torqueValues,
            PointGeometry = null
        },
      };
      Labels = new[] { 50, 100, 150, 200, 250 };
      YFormatter = value => value.ToString();
      DataContext = this;
    }

  }
}
