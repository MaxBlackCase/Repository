using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Threading;

namespace MyAppModBus.Controllers
{
  internal class ControllerBase
  {

    private DispatcherTimer _timer = null;
    private SerialPort _serial = null;
    private ModbusSerialMaster _master;
    const byte slaveID = 1;
    const ushort startAddress = 0;
    const ushort numburOfPoints = 18;


    public ControllerBase()
    {

    }

    /// <summary>
    /// Инициализация массива портов
    /// </summary>
    /// <param name="_portName">Имя Порта</param>
    /// <returns></returns>
    internal void AddItemToComboBox(ref List<string> _portList)
    {
      //Получение портов
      string[] ports = SerialPort.GetPortNames();
      _portList = new List<string>();

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
    }
    internal string ConnectToDevice(string _portName, string _errMessage)
    {
      _serial = new SerialPort();
      try
      {
        if (_serial.IsOpen)
        {
          _serial.Close();
          //disconnectComPort.Visibility = Visibility.Hidden;
        }
        #region <Настройки RTU подключения>
        _serial.PortName = _portName;
        _serial.BaudRate = 19200;
        _serial.Parity = Parity.None;
        _serial.StopBits = StopBits.One;
        _serial.DtrEnable = true;
        _serial.Open();
        #endregion
        _master = ModbusSerialMaster.CreateRtu(_serial);

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

        //_propsArgs = Visibility.Hidden;
        _errMessage = string.Format("{0} порт подключен", _portName);

      }
      catch (Exception err)
      {
        _errMessage = err.Message;
      }
      return _errMessage;

    }

    internal string DisconnectToDevice(string _portName, string _errMessage)
    {
      if (_serial.IsOpen)
      {
        _serial.Close();
        _serial.Dispose();
        _errMessage = string.Format("Порт {0} закрыт", _portName);
      }

      return _errMessage;
    }

    internal bool SetElementEnable(bool _elemEnable)
    {
      if (_elemEnable == true && _serial.IsOpen)
      {
        _elemEnable = false;
      }
      else
      {
        _elemEnable = true;
      }
      return _elemEnable;
    }
    internal bool SetElementDisable(bool _elemDisable)
    {
      if (_elemDisable == true && _serial.IsOpen)
      {
        _elemDisable = false;
      }
      else
      {
        _elemDisable = true;
      }
      return _elemDisable;
    }

    internal string SetElementVisible(string _elemVis)
    {
      if (_elemVis == Visibility.Visible.ToString() && _serial.IsOpen)
      {
        _elemVis = Visibility.Hidden.ToString();
      }
      else
      {
        _elemVis = Visibility.Visible.ToString();
      }
      return _elemVis;
    }
    internal string SetElementHidden(string _elemHid)
    {
      if (_elemHid == Visibility.Hidden.ToString() && _serial.IsOpen)
      {
        _elemHid = Visibility.Visible.ToString();
      }
      else
      {
        _elemHid = Visibility.Hidden.ToString();
      }
      return _elemHid;
    }

  }

}

