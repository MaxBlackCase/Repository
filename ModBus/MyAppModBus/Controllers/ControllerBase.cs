using Modbus.Device;
using MyAppModBus.ViewModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Threading;

namespace MyAppModBus.Controllers
{
  internal class ControllerBase
  {

    private DispatcherTimer _timer = new DispatcherTimer();
    private SerialPort _serialPort = null;
    private ModbusSerialMaster _master;
    const byte slaveID = 1;
    const ushort startAddress = 0;
    const ushort numburOfPoints = 18;


    private SfChartViewModel vm = new SfChartViewModel();

    public ControllerBase()
    {

    }

    /// <summary>
    /// Инициализация массива портов
    /// </summary>
    /// <param name="_portName">Имя Порта</param>
    /// <returns></returns>
    internal List<string> AddItemToComboBox(List<string> _portList)
    {
      //Получение портов
      string[] ports = SerialPort.GetPortNames();
      for (var i = 0; i < ports.Length; i++)
      {
        if (ports[i] != null)
        {
          string str = ports[i].ToString();
          int maxLength = 3;
          string result = str.Substring(0, Math.Min(str.Length, maxLength));
          if (result == "COM")
          {
            _portList.Add(ports[i]);
          }
        }
        else
        {
          _portList.Add("Порт Отсутствует");
        }
      }
      return _portList;
    }

    internal SerialPort ConnectToDevice(SerialPort _serial)
    {
      try
      {
        if (_serialPort.IsOpen)
        {
          _serialPort.Close();
          //disconnectComPort.Visibility = Visibility.Hidden;
        }
        #region <Настройки RTU подключения>
        _serial.PortName = vm.PortList.ToString();
        _serial.BaudRate = 119200;
        _serial.Parity = Parity.None;
        _serial.StopBits = StopBits.One;
        _serial.ReadTimeout = vm.ReadWriteTimeOut;
        _serial.WriteTimeout = vm.ReadWriteTimeOut;
        _serial.DtrEnable = true;
        _serial.Open();
        #endregion

        _master = ModbusSerialMaster.CreateRtu(_serialPort);

        #region UIElements
        //uiElements[0] = new UIElement[] { checkBoxWrite_1, checkBoxWrite_2, checkBoxWrite_3, StartRegsRequest };
        //uiElements[1] = new UIElement[] { decTextBox, decButtonTimeout, comboBoxMainPorts };


        //for (int i = 0; i < 1; i++)
        //{
        //  foreach (var tr in uiElements[0])
        //  {
        //    tr.IsEnabled = true;
        //  }
        //  foreach (var fls in uiElements[1])
        //  {
        //    fls.IsEnabled = false;
        //  }
        //}
        #endregion


        vm.VisibilityButton = "Hidden";
        vm.VisibilityButton = "Visible";
        vm.ErrMessage = $"Порт {_serialPort.PortName} Подключен";

      }
      catch (Exception err)
      {
        vm.ErrMessage = $"Ошибка: {err.Message}";
      }

      return _serial;
    }

  }

}
