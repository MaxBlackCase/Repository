using Modbus.Device;
using MyAppModBus.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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
      private int _readWriteConvert = 50;
      private ObservableCollection<ViewRegisterModel> _viewRegs;

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
         //_readWriteConvert = ConvertToInt(_readWriteConvert, _errMessage);
         try
         {
            if (_serial.IsOpen)
            {
               _serial.Close();
            }
            #region <Настройки RTU подключения>
            _serial.PortName = _portName;
            _serial.ReadTimeout = _readWriteConvert;
            _serial.WriteTimeout = _readWriteConvert;
            _serial.BaudRate = 19200;
            _serial.Parity = Parity.None;
            _serial.StopBits = StopBits.One;
            _serial.DtrEnable = true;
            _serial.Open();
            #endregion
            _master = ModbusSerialMaster.CreateRtu(_serial);

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

      internal double countTime = 0;
      internal double countIndex = 0;
      private int[][] _numberRegisters = new int[2][];

      private DateTime _tSpan;

      internal ObservableCollection<string> GetRegisterToDevice(ObservableCollection<string> _registers, string _readWrite, ref string _errMessage)
      {
         ushort[] result = _master.ReadHoldingRegisters(slaveID, startAddress, numburOfPoints);

         try
         {
            countTime += Convert.ToDouble( _readWrite );
            _tSpan = DateTime.Now;
            _viewRegs = new ObservableCollection<ViewRegisterModel>();
            ///Вывод всех регистров на экран
            for (int i = 0; i < result.Length; i++)
            {
               _viewRegs.Add(new ViewRegisterModel { ID = i, Value = result[i].ToString() });
               //textViewer.Text += $"Регистр: {i} \t{result[i]}\n";
            }
            foreach (var item in _viewRegs)
            {
               _registers.Add($"Регистр: {item.ID}  |  {item.Value}");
            }
            #region Code <->
            //Запуск функции отображения концевиков
            //SetValSingleRegister(result[9], result[10]);

            ///Занесение значений на график
            //if (countTime % _readWrite == 0)
            //{
            //   #region OldGraphs
            //   //for ( int valueFirstChart = 0; valueFirstChart < _arrDict[ 0 ].Count(); valueFirstChart++ ) {
            //   //  _arrDict[ 0 ][ valueFirstChart ].Add( countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 0 ][ valueFirstChart ] ] ) );
            //   //  //Очищение коллекции точек График 1
            //   //  if (_arrDict[0][valueFirstChart].Count > 1000) { _arrDict[0][valueFirstChart].Clear(); }
            //   //}
            //   //for ( int valueSecondChart = 0; valueSecondChart < _arrDict[ 1 ].Count(); valueSecondChart++ ) {
            //   //  _arrDict[ 1 ][ valueSecondChart ].Add(countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 1 ][ valueSecondChart ] ] ) );
            //   //  //Очищение коллекции точек График 2
            //   //  if (_arrDict[1][valueSecondChart].Count > 1000){_arrDict[1][valueSecondChart].Clear(); }
            //   //}

            //   //for ( int valueFirstChart = 0; valueFirstChart < _linesArr[ 0 ].Length; valueFirstChart++ ) {
            //   //  _linesArr[ 0 ][ valueFirstChart ].Plot( _arrDict[ 0 ][ valueFirstChart ].Keys, _arrDict[ 0 ][ valueFirstChart ].Values );
            //   //}
            //   //for ( int valueSecondChart = 0; valueSecondChart < _linesArr[ 1 ].Length; valueSecondChart++ ) {
            //   //  _linesArr[ 1 ][ valueSecondChart ].Plot( _arrDict[ 1 ][ valueSecondChart ].Keys, _arrDict[ 1 ][ valueSecondChart ].Values );
            //   //}
            //   #endregion

            //   #region NewGraphs
            //   #endregion

            //}
            #endregion
            countIndex++;
         }
         catch (Exception err)
         {
            _errMessage = err.Message;
         }
         return _registers;

      }

      internal string RegistersRequest( string _queryRegisters)
      {
         try
         {
            if (_serial.IsOpen && _queryRegisters == "Off")
            {
               #region <Timer>
               //_timer.Tick += new EventHandler(GetRegisterToDevice);
               //_timer.Interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(_readWrite));
               //_timer.Start();
               _queryRegisters = "On";
               #endregion
            }
            else
            {
               //_timer.Stop();
               _queryRegisters = "Off";
            }
         }
         catch (Exception err)
         {
            //_errMessage.Text = $"Ошибка: {err.Message}";
         }
         return _queryRegisters;
      }

      internal string ConvertToInt(string _readWrite, string _errMessage)
      {

         if (_readWrite != null)
         {
         _readWriteConvert = Convert.ToInt32(_readWrite);
            if (_readWriteConvert < 20)
            {
               _readWriteConvert = 20;
               _errMessage = string.Format("Интервал не может быть меньше {0} ms, поэтому задан интервал по умолчанию {0} ms.",_readWriteConvert);
            }
            else if (_readWriteConvert > 100)
            {
               _readWriteConvert = 100;
               _errMessage = string.Format("Значение не может превышать значение в {0} ms, поэтому задано значение по умолчанию {0} ms.", _readWriteConvert);
            }
            else
            {
               _errMessage = string.Format("Значение интервала опроса устроства: {0} ms", _readWriteConvert);
            }
         }
         return _errMessage;
      }

      internal string CheckReadWriteOnRegular(string _pullTimeString)
      {

         string pattern = @"^[0-9]";

         while (true)
         {
            if (Regex.IsMatch(_pullTimeString, pattern, RegexOptions.IgnoreCase))
            {
               _pullTimeString = string.Format("{0} ms.", _pullTimeString);
               break;
            }
            else
            {
               _pullTimeString = "Error";
            }
         }
         return _pullTimeString;
      }
   }

}

