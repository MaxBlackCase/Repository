using Modbus.Device;
using MyAppModBus.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyAppModBus.Controllers {
  internal class ControllerBase {

    private DispatcherTimer _timer = new DispatcherTimer();
    private SerialPort _serial = new SerialPort();
    private ModbusSerialMaster _master;
    const byte slaveID = 1;
    private string _stateSerialPort = null;
    private string _errMessage;
    const ushort startAddress = 0;
    const ushort numburOfPoints = 18;
    private int _readWriteConvert = 50;
    private string _queryRegisters;
    private ObservableCollection<ViewRegisterModel> _viewRegs = new ObservableCollection<ViewRegisterModel>();
    private ObservableCollection<string> _registers = new ObservableCollection<string>();
    private Ellipse _ellipseFittings;
    private ObservableCollection<Ellipse> _clnEllipseFittings = new ObservableCollection<Ellipse>();

    #region LineSeriesCollection
    private ObservableCollection<ChartPoints> _volt = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _curr = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _torq = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _external = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _motor = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints>[] _arrSerires = new ObservableCollection<ChartPoints>[ 5 ];
    #endregion


    public ControllerBase() {

    }

    /// <summary>
    /// Инициализация массива портов
    /// </summary>
    /// <param name="_portName">Имя Порта</param>
    /// <returns></returns>
    internal void AddItemToComboBox( ref List<string> _portList ) {
      //Получение портов
      string[] ports = SerialPort.GetPortNames();
      _portList = new List<string>();

      for( var i = 0; i < ports.Length; i++ ) {
        if( ports[ i ] != null ) {
          string str = ports[ i ].ToString();
          int maxLength = 3;
          string result = str.Substring( 0, Math.Min( str.Length, maxLength ) );
          if( result == "COM" ) {
            _portList.Add( ports[ i ] );
          }
        }
        else {
          _portList.Add( "Порт Отсутствует" );
        }
      }
    }

    /// <summary>
    /// Соединение с устройством
    /// </summary>
    /// <param name="_portName">Имя Порта</param>
    /// <param name="_errMessage">Сообщение</param>
    /// <returns></returns>
    internal (string, string, string) ConnectToDevice( string _portName ) {
      try {
        if( !_serial.IsOpen ) {
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
          _master = ModbusSerialMaster.CreateRtu( _serial );
          _errMessage = string.Format( "{0} порт подключен", _portName );
          _stateSerialPort = "Отключить";
          _queryRegisters = "Start";
        }
        else {
          DisconnectToDevice();
          _errMessage = string.Format( "{0} порт Отключен", _portName );
        }
      }
      catch( Exception err ) {
        _errMessage = err.Message.ToString();
        _stateSerialPort = "Подключить";
      }
      return (_errMessage, _stateSerialPort, _queryRegisters);
    }

    /// <summary>
    /// Отключение от устройства
    /// </summary>
    /// <param name="_portName">Имя порта</param>
    /// <param name="_errMessage">Сообщение</param>
    /// <returns></returns>
    internal void DisconnectToDevice() {
      if( _serial.IsOpen && _timer.IsEnabled ) {
        // <Сброс регистров>
        var cleanRegs = new ushort[ 3 ] { 6, 7, 8 };
        for( int i = 0; i < cleanRegs.Length; i++ ) {
          _master.WriteSingleRegister( slaveID, cleanRegs[ i ], 0 );
        }
        //</Сброс регистров>
      }
      _countTimes = 0;
      foreach( var item in _arrSerires ) {
        if( item != null ) {
          item.Clear();
        }
      }

      _timer.Stop();
      _serial.Close();
      _serial.Dispose();
      _master.Dispose();
      _errMessage = "Остановлено";
      _stateSerialPort = "Подключить";
      _queryRegisters = "Start";
    }

    #region Elements IsEnable, IsVisibility
    /// <summary>
    /// Переключатель активности для елементов до подключения к устройству
    /// </summary>
    /// <param name="_elemEnable">Значение переключателя</param>
    /// <returns></returns>
    internal bool SetElementEnable( bool _elemEnable ) {
      if( _elemEnable == true && _serial.IsOpen ) {
        _elemEnable = false;
      }
      else {
        _elemEnable = true;
      }
      return _elemEnable;
    }
    internal bool SetElementDisable( bool _elemDisable ) {
      if( _elemDisable == false && _serial.IsOpen ) {
        _elemDisable = true;
      }
      else {
        _elemDisable = false;
      }
      return _elemDisable;
    }
    /// <summary>
    /// Переключатель отображения для елементов до подключения к устройству
    /// </summary>
    /// <param name="_elemEnable">Значение переключателя</param>
    /// <returns></returns>
    internal string SetElementVisible( string _elemVis ) {
      if( _elemVis == Visibility.Visible.ToString() && _serial.IsOpen ) {
        _elemVis = Visibility.Hidden.ToString();
      }
      else {
        _elemVis = Visibility.Visible.ToString();
      }
      return _elemVis;
    }
    //internal string SetElementHidden( string _elemHid ) {
    //  if (_elemHid == Visibility.Hidden.ToString() && _serial.IsOpen) {
    //    _elemHid = Visibility.Visible.ToString();
    //    } else {
    //    _elemHid = Visibility.Hidden.ToString();
    //    }
    //  return _elemHid;
    //  }
    #endregion

    private int _countTimes = 0;
    /// <summary>
    /// Получение данных из регистров
    /// </summary>
    /// <param name="sender">Объект</param>
    /// <param name="e">Событие</param>
    private void GetRegisterToDevice( object sender, EventArgs e ) {
      _viewRegs.Clear();
      _registers.Clear();
      _clnEllipseFittings.Clear();
      _countTimes += _readWriteConvert;
      ///Вывод всех регистров на экран
      try {
        if( _serial.IsOpen ) {
          ushort[] result = _master.ReadHoldingRegisters( slaveID, startAddress, numburOfPoints );
          for( int i = 0; i < result.Length; i++ ) {
            _viewRegs.Add( new ViewRegisterModel { ID = i, Value = result[ i ].ToString() } );
          }
          foreach( var item in _viewRegs ) {
            _registers.Add( $"Регистр: {item.ID}\t|  {item.Value}" );
          }
          SetColorEllipses( result[ 9 ], result[ 10 ] );
          if( _countTimes % _readWriteConvert == 0 ) {
            SetPointsSeries( result[ 0 ], 0, _volt );
            SetPointsSeries( result[ 1 ], 1, _curr );
            SetPointsSeries( result[ 4 ], 4, _torq );
            SetPointsSeries( result[ 2 ], 2, _external );
            SetPointsSeries( result[ 3 ], 3, _motor );
          }
        }
        else {
          _timer.Stop();
        }
      }
      catch( Exception err ) {
        _registers.Add( err.Message.ToString() );
        _timer.Stop();
      }

    }

    //private List<ObservableCollection> lstChartSeries
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_queryRegisters">Значение кнопки</param>
    /// <returns></returns>
    internal (ObservableCollection<string>, string, string, ObservableCollection<Ellipse>, ObservableCollection<ChartPoints>[]) RegistersRequest() {
      try {
        if( _serial.IsOpen ) {
          #region <Timer>
          _timer.Tick += new EventHandler( GetRegisterToDevice );
          _timer.Interval = new TimeSpan( 0, 0, 0, 0, Convert.ToInt32( _readWriteConvert ) );
          _arrSerires[ 0 ] = _volt;
          _arrSerires[ 1 ] = _curr;
          _arrSerires[ 2 ] = _torq;
          _arrSerires[ 3 ] = _external;
          _arrSerires[ 4 ] = _motor;
          if( !_timer.IsEnabled ) {
            _timer.Start();
            _queryRegisters = "Stop";
            _errMessage = "Запущено...";
          }
          else {
            _timer.Stop();
            _queryRegisters = "Start";
            _errMessage = "Остановлено...";
          }
          #endregion
        }
      }
      catch( Exception err ) {
        _errMessage = err.Message.ToString();
      }
      return (_registers, _queryRegisters, _errMessage, _clnEllipseFittings, _arrSerires);
    }
    internal string ConvertToInt( string _readWrite ) {

      if( _readWrite != null ) {
        _readWriteConvert = Convert.ToInt32( _readWrite );
        if( _readWriteConvert < 20 ) {
          _readWriteConvert = 20;
          _errMessage = string.Format( "Интервал не может быть меньше {0} ms, поэтому задан интервал по умолчанию {0} ms.", _readWriteConvert );
        }
        else if( _readWriteConvert > 100 ) {
          _readWriteConvert = 100;
          _errMessage = string.Format( "Значение не может превышать значение в {0} ms, поэтому задано значение по умолчанию {0} ms.", _readWriteConvert );
        }
        else {
          _errMessage = string.Format( "Значение интервала опроса устроства: {0} ms", _readWriteConvert );
        }
      }
      return _errMessage;
    }
    private void SetColorEllipses( ushort valOne, ushort valTwo ) {
      var regFit = new ushort[ 2 ];
      regFit[ 0 ] = valOne;
      regFit[ 1 ] = valTwo;
      for( int i = 0; i < regFit.Length; i++ ) {
        if( regFit[ i ] == 1 ) {
          _ellipseFittings = new Ellipse
          {
            Width = 25,
            Height = 25,
            Fill = Brushes.Green,
            Margin = new Thickness( 5 )
          };
        }
        else {
          _ellipseFittings = new Ellipse
          {
            Width = 25,
            Height = 25,
            Fill = Brushes.Red,
            Margin = new Thickness( 5 )
          };
        }
        _clnEllipseFittings.Add( _ellipseFittings );
      }
    }

    private bool[] elemBool = new bool[ 3 ] { false, false, false };
    internal string WriteValuesToRegisters( object _indTogElem ) {

      _indTogElem = Convert.ToInt32( _indTogElem );
      try {
        if( _serial.IsOpen ) {
          switch( _indTogElem ) {
            case 1:
            if( elemBool[ 0 ] == false ) {
              _master.WriteSingleRegister( slaveID, 6, 1 );
              elemBool[ 0 ] = true;
            }
            else {
              _master.WriteSingleRegister( slaveID, 6, 0 );
              elemBool[ 0 ] = false;
            }
            break;
            case 2:
            if( elemBool[ 1 ] == false ) {
              _master.WriteSingleRegister( slaveID, 7, 1 );
              elemBool[ 1 ] = true;
            }
            else {
              _master.WriteSingleRegister( slaveID, 7, 0 );
              elemBool[ 1 ] = false;
            }
            break;
            case 3:
            if( elemBool[ 2 ] == false ) {
              _master.WriteSingleRegister( slaveID, 8, 1 );
              elemBool[ 2 ] = true;
            }
            else {
              _master.WriteSingleRegister( slaveID, 8, 0 );
              elemBool[ 2 ] = false;
            }
            break;
          }
        }
        else {
          _errMessage = "Запись в регистры не возможна, отсутствует подключение";
        }
      }
      catch( Exception err ) {
        _errMessage = err.Message.ToString();
      }

      return _errMessage;
    }


    /// <summary>
    /// Добавление точки серии в коллекцию
    /// </summary>
    /// <param name="_valRegister">Значение регистра</param>
    /// <param name="_lineSeries">Имя серии</param>
    private void SetPointsSeries( ushort _valRegister,int indexRegistrs, ObservableCollection<ChartPoints> _lineSeries ) {
      var _time = TimeSpan.FromMilliseconds( _countTimes );
     double _value = 0;
      switch( indexRegistrs ) {
        case 0:
        _value = ConverValuesFfromRegisters( _valRegister, 17, 1300, 0.0, 80.0 );
        break;
        case 1:
        _value = ConverValuesFfromRegisters( _valRegister, 5, 46, 0.0, 52.0 );
        break;
        case 4:
        _value = ConverValuesFfromRegisters( _valRegister, 45, 4046, -1000.0, 1000.0 );
        break;
        case 2:
        _value = Convert.ToDouble(_valRegister);
        break;
        case 3:
        _value = Convert.ToDouble( _valRegister );
        break;
      }
      _lineSeries.Add( new ChartPoints { XTime = _time, YValue = _value } );
    }

    private double ConverValuesFfromRegisters( ushort inVal, double inMin, double inMax, double outMin, double outMax ) {

      var x = Convert.ToDouble(inVal) ;
      var result = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
      return  result;
    }

  }
}

