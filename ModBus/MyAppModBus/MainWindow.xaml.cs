using Modbus.Device;
using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.Generic;
using InteractiveDataDisplay.WPF;
using MahApps.Metro.Controls;
using ControlzEx.Theming;

namespace MyAppModBus {
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow {

    const byte slaveID = 1;

    private readonly ushort startAddress = 0;
    private readonly ushort numburOfPoints = 18;
    private int readWriteTimeOut = 50;

    public static string result;
    private DispatcherTimer timer;
    public static SerialPort _serialPort = null;
    public static ModbusSerialMaster master = null;

    private Dictionary<int, double> volltage = new Dictionary<int, double>();
    private Dictionary<int, double> current = new Dictionary<int, double>();
    private Dictionary<int, double> torque = new Dictionary<int, double>();
    private Dictionary<int, double> tempExternal = new Dictionary<int, double>();
    private Dictionary<int, double> tempMotor = new Dictionary<int, double>();

    private LineGraph volltageLine = new LineGraph();
    private LineGraph currentLine = new LineGraph();
    private LineGraph torqueLine = new LineGraph();
    private LineGraph externalLine = new LineGraph();
    private LineGraph motorLine = new LineGraph();


    /// <summary>
    /// Главнео окно
    /// </summary>
    public MainWindow() {
      InitializeComponent();
      AddItemToComboBox();
      ThemeManager.Current.ChangeTheme( this, "Dark.Steel" );
      GraphLines();
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

        //Сброс регистров
        ResetRegisters();

        StartRegsRequest.IsEnabled = true;
        checkBoxWrite_1.IsEnabled = true;
        checkBoxWrite_2.IsEnabled = true;
        checkBoxWrite_3.IsEnabled = true;
        decTextBox.IsEnabled = false;
        decButtonTimeout.IsEnabled = false;
        comboBoxMainPorts.IsEnabled = false;
        connectComPort.Visibility = Visibility.Hidden;
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
      _serialPort.Dispose();

      comboBoxMainPorts.IsEnabled = true;
      disconnectComPort.Visibility = Visibility.Hidden;
      connectComPort.Visibility = Visibility.Visible;
      textViewer.Text = $"Порт {_serialPort.PortName} закрыт";
      decButtonTimeout.IsEnabled = true;
      decTextBox.IsEnabled = true;
      StartRegsRequest.IsEnabled = false;
      StartRegsRequest.Content = "Запустить";
    }

    /// <summary>
    /// Получение данных с регистров
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private double countTime = 0;
    private int countIndex = 0;

    private void GetHoldReg( object sender, EventArgs e ) {
      ushort[] result = master.ReadHoldingRegisters( slaveID, startAddress, numburOfPoints );
      try {

        textViewer.Text = "";
        countTime += readWriteTimeOut;

        for ( int i = 0; i < result.Length; i++ ) {
          textViewer.Text += $"Регистр: {i} \t{result[ i ]}\n";

        }

        SetValSingleRegister( result[ 9 ], result[ 10 ] );

        if ( countTime % readWriteTimeOut == 0 ) {
          volltage.Add( countIndex, Convert.ToDouble( result[ 0 ] ) );
          current.Add( countIndex, Convert.ToDouble( result[ 1 ] ) );
          torque.Add( countIndex, Convert.ToDouble( result[ 4 ] ) );
          tempExternal.Add( countIndex, Convert.ToDouble( result[ 2 ] ) );
          tempMotor.Add( countIndex, Convert.ToDouble( result[ 3 ] ) );

          volltageLine.Plot( volltage.Keys, volltage.Values );
          currentLine.Plot( current.Keys, current.Values );
          torqueLine.Plot( torque.Keys, torque.Values );
          externalLine.Plot( tempExternal.Keys, tempExternal.Values );
          motorLine.Plot( tempMotor.Keys, tempMotor.Values );

        }

        countIndex++;
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
                Margin = new Thickness( 0, 0, 10, 15 )
              };
              LimSwPanel.Children.Add( LimSwEllipse );
            }
            else {
              Ellipse LimSwEllipse = new Ellipse
              {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                Margin = new Thickness( 0, 0, 10, 15 )
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


    /// <summary>
    /// Отрисовка графиков и их линий
    /// </summary>
    private void GraphLines() {

      lines_one.Children.Add( volltageLine );
      lines_one.Children.Add( currentLine );
      lines_one.Children.Add( torqueLine );
      lines_two.Children.Add( externalLine );
      lines_two.Children.Add( motorLine );

      //Линии первого графика
      volltageLine.Stroke = new SolidColorBrush( Color.FromRgb( 33, 150, 243 ) );
      volltageLine.Description = String.Format( $"Voltage" );
      volltageLine.StrokeThickness = 2;
      currentLine.Stroke = new SolidColorBrush( Color.FromRgb( 76, 175, 80 ) );
      currentLine.Description = String.Format( $"Current" );
      currentLine.StrokeThickness = 2;
      torqueLine.Stroke = new SolidColorBrush( Color.FromRgb( 251, 140, 0 ) );
      torqueLine.Description = String.Format( $"Torque" );
      torqueLine.StrokeThickness = 2;

      //Линии второго графика
      externalLine.Stroke = new SolidColorBrush( Color.FromRgb( 244, 67, 54 ) );
      externalLine.Description = String.Format( $"Temp Extermal" );
      externalLine.StrokeThickness = 2;

      motorLine.Stroke = new SolidColorBrush( Color.FromRgb( 103, 58, 183 ) );
      motorLine.Description = String.Format( $"Temp Motor" );
      motorLine.StrokeThickness = 2;

    }

    /// <summary>
    /// Запрос из регистров Master(a) и запусе/остановка таймера
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RegistersRequest( object sender, RoutedEventArgs e ) {
      var BtnStartTimerAndRegistersRequest = StartRegsRequest;
      try {
        if ( _serialPort.IsOpen && timer.IsEnabled == false ) {
          #region <Timer>
          timer.Tick += new EventHandler( GetHoldReg );
          timer.Interval = new TimeSpan( 0, 0, 0, 0, readWriteTimeOut );
          timer.Start();
          #endregion
          BtnStartTimerAndRegistersRequest.Content = "Остановить";
        }
        else {
          timer.Stop();
          BtnStartTimerAndRegistersRequest.Content = "Запустить";
        }
      }
      catch ( Exception err ) {
        textViewer.Text = $"Ошибка: {err.Message}";
      }
    }

    /// <summary>
    /// Запись в регистры
    /// </summary>
    /// <param name="sender">Объект</param>
    /// <param name="e">Событие</param>
    private void CheсkValToRegisters( object sender, RoutedEventArgs e ) {
      ToggleSwitch toggleSwitch = sender as ToggleSwitch;
      var indElem = CheckBoxWriteRegisters.Children.IndexOf( toggleSwitch );
      ushort[] arrRegisters = new ushort[] { 6, 7, 8 };
      if ( toggleSwitch != null && _serialPort.IsOpen ) {
        if ( toggleSwitch.IsOn == true ) {
          for ( var i = 0; i < arrRegisters.Length; i++ ) {
            if ( i == indElem ) {
              master.WriteSingleRegister( slaveID, arrRegisters[ i ], 1 );
            }
          }
        }
        else {
          for ( var i = 0; i < arrRegisters.Length; i++ ) {
            if ( i == indElem ) {
              master.WriteSingleRegister( slaveID, arrRegisters[ i ], 0 );
            }
          }
        }
      }
    }


    //private void ZoomUpSl_ValueChanged( object sender, RoutedEventArgs e ) {
    //  PlotUp.PlotOriginY = (50 * ZoomUpSl.UpperValue) - 50;
    //  PlotUp.PlotHeight = -(50 * (100 - ZoomUpSl.LowerValue));
    //}


  }
}
