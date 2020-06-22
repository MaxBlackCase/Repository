//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.IO.Ports;
//using System.Threading;
//using System.Net.Sockets;
//using Modbus.IO;
//using Modbus.Device;

//namespace ModBus {
//  /// <summary>
//  /// Логика взаимодействия для MainWindow.xaml
//  /// </summary>
//  public partial class MainWindow : Window {
//    public MainWindow() {
//      InitializeComponent();
//      addItemToComboBox();
//    }

//    //Инициализация портов
//    private void addItemToComboBox() {
//      //Получение портов
//      string[] ports = SerialPort.GetPortNames();
//      foreach ( string port in ports ) {
//        if ( port == "" ) {
//          comboBoxMainPorts.Items.Add( "Отсутствует порт" );
//        }
//        else {
//          string str = port.ToString();
//          int maxLength = 3;
//          string result = str.Substring( 0, Math.Min( str.Length, maxLength ) );

//          if ( result == "COM" ) {
//            comboBoxMainPorts.Items.Add( port );
//          }
//        }
//      }
//      comboBoxMainPorts.SelectedIndex = 0;
//    }


//    //Подключение к устройству по RTU
//    SerialPort srlPort = new SerialPort();

//    private void connectToDevice( object sender, RoutedEventArgs e ) {
//      try {
//        srlPort.PortName = comboBoxMainPorts.Text;
//        srlPort.BaudRate = 19200;
//        srlPort.DataBits = 8;
//        srlPort.Parity = Parity.None;
//        srlPort.StopBits = StopBits.One;
//        srlPort.ReadTimeout = 300;
//        srlPort.WriteTimeout = 300;
//        srlPort.Open();

//        if ( srlPort.IsOpen ) {
//          ModbusSerialMaster master = ModbusSerialMaster.CreateRtu( srlPort );
//          byte slaveID = 11;
//          string one = "0x006B";
//          string two = "0x0003";
//          //ushort startAdress = Convert.ToUInt16(one, 16);
//          //ushort numOfPoints = Convert.ToUInt16(two, 16);
//          //ushort[] holding_register = master.ReadHoldingRegisters(slaveID, startAdress, numOfPoints);

//          //foreach(ushort i in holding_register)
//          //{
//          //  txtBlockPort.Text = i.ToString();
//          //}

//          connectButton.Visibility = Visibility.Collapsed;
//          disconBtn.Visibility = Visibility.Visible;
//          txtBlockPort.Text = srlPort.PortName + " port - Is Open";
//        }
//        else {
//          srlPort.Close();
//          txtBlockPort.Text = "Выберите порт";
//        }
//      }
//      catch ( Exception err ) {
//        txtBlockPort.Text = err.ToString();
//        connectButton.Visibility = Visibility.Collapsed;
//        disconBtn.Visibility = Visibility.Visible;
//        return;
//      }


//    }
//    private void disconnectBtn( object sender, RoutedEventArgs e ) {
//      srlPort.Close();
//      disconBtn.Visibility = Visibility.Collapsed;
//      connectButton.Visibility = Visibility.Visible;
//      txtBlockPort.Text = String.Empty;
//    }

//  }


//}
