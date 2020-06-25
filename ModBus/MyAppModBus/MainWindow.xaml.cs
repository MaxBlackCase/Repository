using Modbus.Device;
using Modbus.Utility;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace MyAppModBus {
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    const byte slaveID = 1;
    private DispatcherTimer timer;
    public static string result;
    public static SerialPort _serialPort = null;

    public MainWindow() {
      InitializeComponent();
      addItemToComboBox();
      btnGetHoldReg.IsEnabled = false;
    }

    public void loadSerialPort() {


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
    /// Открытие закрытие COM - порта
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
        _serialPort.PortName = comboBoxMainPorts.Text;
        _serialPort.BaudRate = 119200;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = 100;
        _serialPort.WriteTimeout = 300;
        _serialPort.DtrEnable = true;
        _serialPort.Open();
        timer.Tick += new EventHandler( getHoldReg );
        timer.Interval = new TimeSpan( 0, 0, 1 );
        timer.Start();

        btnGetHoldReg.IsEnabled = true;
        comboBoxMainPorts.IsEnabled = false;
        disconnectComPort.Visibility = Visibility.Visible;
        textViewer.Text = $"Порт {_serialPort.PortName} подключен";


      }
      catch ( Exception ex ) {
        connectComPort.Content = "Подкл";
        _serialPort.Close();
        textViewer.Text += ex.Message;

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

    public void getHoldReg( object sender, EventArgs e ) {


      try {
        //byte funcCode = 3;
        ushort startAddr = 0;
        ushort numOfPoints = 18;

        ModbusSerialMaster master = ModbusSerialMaster.CreateRtu( _serialPort );

        ushort[] result = master.ReadHoldingRegisters( slaveID, startAddr, numOfPoints );

        textViewer.Text = "";
        int i = 0;
        foreach ( ushort item in result ) {

          textViewer.Text += $"Регистр: {i} \t{item}\n";
          
          i++;
        }
      }
      catch ( Exception err ) {

        textViewer.Text = $"Error: {err.Message}";
      }


      #region Контрольная сумма
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

    }

    /// <summary>
    /// Подсчет контрольной суммы
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
}
