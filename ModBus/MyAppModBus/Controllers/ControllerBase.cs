using MyAppModBus.ViewModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows.Documents;

namespace MyAppModBus.Controllers
{
  internal class ControllerBase
  {

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
  }

}
