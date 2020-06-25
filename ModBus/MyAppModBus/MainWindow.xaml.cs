using Modbus.Device;
using System;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MyAppModBus {
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    const byte slaveID = 1;
    private ushort startAddress = 0;
    private ushort numburOfPoints = 18;
    private DispatcherTimer timer;
    public static string result;
    public static SerialPort _serialPort = null;
    public static ModbusSerialMaster master = null;

    public MainWindow() {
      InitializeComponent();
      addItemToComboBox();
      btnGetHoldReg.IsEnabled = false;

    }

    //Инициализация портов
    private void addItemToComboBox() {
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
    public void connectToDevice( object sender, RoutedEventArgs e ) {
      _serialPort = new SerialPort();
      timer = new DispatcherTimer();
      try {
        if ( _serialPort.IsOpen ) {
          _serialPort.Close();
          disconnectComPort.Visibility = Visibility.Hidden;
          btnGetHoldReg.IsEnabled = false;
        }

        #region <Настройки RTU подключения>
        _serialPort.PortName = comboBoxMainPorts.Text;
        _serialPort.BaudRate = 119200;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = 1000;
        _serialPort.WriteTimeout = 300;
        _serialPort.DtrEnable = true;
        _serialPort.Open();
        #endregion

        master = ModbusSerialMaster.CreateRtu( _serialPort );

        timer.Tick += new EventHandler( getHoldReg );
        timer.Interval = new TimeSpan( 0, 0, 1 );
        timer.Start();

        btnGetHoldReg.IsEnabled = true;
        comboBoxMainPorts.IsEnabled = false;
        disconnectComPort.Visibility = Visibility.Visible;
        textViewer.Text = $"Порт {_serialPort.PortName} подключен";

      }
      catch ( Exception err ) {
        connectComPort.Content = "Подкл";
        _serialPort.Close();
        textViewer.Text = $"Ошибка: {err.Message}";

      }


    }

    private void disconnectToDevice( object sender, RoutedEventArgs e ) {
      timer.Stop();
      _serialPort.Close();
      comboBoxMainPorts.IsEnabled = true;
      disconnectComPort.Visibility = Visibility.Hidden;
      textViewer.Text = $"Порт {_serialPort.PortName} закрыт";
      btnGetHoldReg.IsEnabled = false;
    }

    private void getHoldReg( object sender, EventArgs e ) {


      try {


        master = ModbusSerialMaster.CreateRtu( _serialPort );
        ushort[] result = master.ReadHoldingRegisters( slaveID, startAddress, numburOfPoints );

        textViewer.Text = "";
        int i = 0;
        foreach ( ushort item in result ) {

          textViewer.Text += $"Регистр: {i} \t{item}\n";

          i++;
        }

        
        SetValSingleRegister();
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
    private void SetValSingleRegister() {

      try {
        if ( _serialPort.IsOpen ) {

          ushort[] res = master.ReadHoldingRegisters( slaveID, startAddress, numburOfPoints );
          int[] arrLimitSwitch = new int[ 2 ];
          arrLimitSwitch[ 0 ] = Convert.ToInt32( res[ 9 ] );
          arrLimitSwitch[ 1 ] = Convert.ToInt32( res[ 10 ] );

          if(arrLimitSwitch[0] == 1 ) {
            LimitSwitch_1.Fill = Brushes.Green;
          }
          else {
            LimitSwitch_1.Fill = Brushes.Red;
          }
          if(arrLimitSwitch[1] == 1 ) {
            LimitSwitch_2.Fill = Brushes.Green;
          }
          else {
            LimitSwitch_2.Fill = Brushes.Red;
          }
        }

      }
      catch ( Exception err ) {

        textViewer.Text = $"Ошибка: {err.Message}";
      }

    }


  }
}
