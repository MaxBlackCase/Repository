using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Modbus.Device;
using MyAppModBus.Controllers;
using MyAppModBus.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Documents;
using System.Windows.Forms;
using TestDataGenerator;

namespace MyAppModBus.ViewModel
{

  internal class SfChartViewModel : ViewModelBase
  {

    private ControllerBase ctr = new ControllerBase();
    private List<string> _portList = new List<string>();
    private int _readWriteTimeOut;
    private string _errMessage;
    private string _visibilityButton;
    private SerialPort _serialPort;
    #region Свойства

    /// <summary>
    /// Список портов
    /// </summary>
    public List<string> PortList
    {
      get => _portList;
      set => Set(ref _portList, value);
    }

    public int ReadWriteTimeOut
    {
      get => _readWriteTimeOut;
      set => Set(ref _readWriteTimeOut, value);
    }

    public string ErrMessage
    {
      get => _errMessage;
      set => Set(ref _errMessage, value);
    }

    public string VisibilityButton
    {
      get => _visibilityButton;
      set => Set(ref _visibilityButton, value);
    }

    public SerialPort SerialPort
    {
      get => _serialPort;
      set => Set(ref _serialPort, value);
    }
    


    #endregion

    public SfChartViewModel()
    {
      ctr.AddItemToComboBox(PortList);
      ctr.ConnectToDevice(SerialPort);
    }

  }
}
