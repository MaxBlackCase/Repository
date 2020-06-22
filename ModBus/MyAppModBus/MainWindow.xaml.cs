﻿using Modbus.Device;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MyAppModBus {
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    static SerialPort _serialPort;
    static bool _continue;

    public MainWindow() {
      InitializeComponent();
      addItemToComboBox();
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
    private void connectToDevice( object sender, RoutedEventArgs e ) {
      _serialPort = new SerialPort();

      try {
        if ( _serialPort.IsOpen ) {
          _serialPort.Close();
          disconnectComPort.Visibility = Visibility.Hidden;
        }
        _serialPort.PortName = comboBoxMainPorts.Text;
        _serialPort.BaudRate = 19200;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;

        _serialPort.ReadTimeout = 300;
        _serialPort.WriteTimeout = 300;

        _serialPort.Open();
        ModbusSerialMaster master = ModbusSerialMaster.CreateRtu( _serialPort );

        disconnectComPort.Visibility = Visibility.Visible;
        textViewer.Text = $"Порт {_serialPort.PortName} подключен";
      }
      catch ( Exception ex ) { 
        connectComPort.Content = "Подкл";
        _serialPort.Close();
        textViewer.Text = ex.Message;

      }


    }

    private void disconnectToDevice(object sender, RoutedEventArgs e ) {
      _serialPort.Close();
      disconnectComPort.Visibility = Visibility.Hidden;
      textViewer.Text = $"Порт {_serialPort.PortName} закрыт";
    }


  }
}
