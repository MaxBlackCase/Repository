using MahApps.Metro.Controls;
using Modbus.Device;
using MyAppModBus.Context;
using MyAppModBus.Models;
using MyAppModBus.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyAppModBus.Controllers {
  internal class ControllerBase {

    private readonly DispatcherTimer _timer = new DispatcherTimer();
    private readonly SerialPort _serial = new SerialPort();
    private ModbusSerialMaster _master;
    const byte slaveID = 1;
    private string _stateSerialPort = null;
    private string _errMessage;
    const ushort startAddress = 0;
    const ushort numburOfPoints = 18;
    private int _readWriteConvert;
    private int _baseMinValReadWrite = 10;
    private int _baseMaxValReadWrite = 1000;
    private int _countTimes = 0;
    private TimeSpan _dTime = new TimeSpan(0,0,0,0,0);
    private string _queryRegisters;
    private string _elemVisible;
    private ObservableCollection<ViewRegisterModel> _viewRegs = new ObservableCollection<ViewRegisterModel>();
    private ObservableCollection<string> _registers = new ObservableCollection<string>();
    private Ellipse _ellipseFittings;
    private ObservableCollection<Ellipse> _clnEllipseFittings = new ObservableCollection<Ellipse>();
    private string _cleanSeries;
    private List<LineGroup> linePointGroup;
    private List<LinePoint> linePoints = new List<LinePoint>();

    #region LineSeriesCollection
    private ObservableCollection<ChartPoints> _volt = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _curr = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _torq = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _external = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints> _motor = new ObservableCollection<ChartPoints>();
    private ObservableCollection<ChartPoints>[] _arrSerires = new ObservableCollection<ChartPoints>[ 5 ];
    #endregion
    public ControllerBase() {

      linePointGroup = new List<LineGroup>() {
        new LineGroup { NameLine = "Volltage" },
        new LineGroup { NameLine = "Current" },
        new LineGroup { NameLine = "Torque" },
        new LineGroup { NameLine = "External" },
        new LineGroup { NameLine = "Motor" }
        };
      SetDbSeriesLine();
      DeletedFromTable();
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
    internal (string, string, string, bool, string) ConnectToDevice( string _portName ) {
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
          _clearBtn = true;
          _elemVisible = SetElementVisible( true );

        }
        else {
          _elemVisible = SetElementVisible( false );
          DisconnectToDevice();
          _errMessage = string.Format( "{0} порт Отключен", _portName );
        }
      }
      catch( Exception err ) {
        _errMessage = err.Message.ToString();
        _stateSerialPort = "Подключить";
      }
      return (_errMessage, _stateSerialPort, _queryRegisters, _clearBtn, _elemVisible);
    }
    /// <summary>
    /// Отключение от устройства
    /// </summary>
    /// <param name="_portName">Имя порта</param>
    /// <param name="_errMessage">Сообщение</param>
    /// <returns></returns>
    internal async void DisconnectToDevice() {
      _errMessage = "Остановлено";
      _stateSerialPort = "Подключить";
      _queryRegisters = "Start";
      _clearBtn = false;
      if( _serial.IsOpen && _timer.IsEnabled ) {
        await Task.Run( () => {
          // <Сброс регистров>
          var cleanRegs = new ushort[ 3 ] { 6, 7, 8 };
          for( int i = 0; i < cleanRegs.Length; i++ ) {
            _master.WriteSingleRegister( slaveID, cleanRegs[ i ], 0 );
          }
        } );
      }
      await Task.Run( () => {
        _timer.Stop();
        _serial.Close();
        _serial.Dispose();
        _master.Dispose();
      } );
      CleanSeriesWithChart();
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
    internal string SetElementVisible( bool _boolState ) {
      string visible = null;
      if( _boolState && _serial.IsOpen ) {
        visible = Visibility.Visible.ToString();
      }
      else {
        visible = Visibility.Hidden.ToString();
      }
      return visible;
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
    /// <summary>
    /// Получение данных из регистров
    /// </summary>
    /// <param name="sender">Объект</param>
    /// <param name="e">Событие</param>
    private void GetRegisterToDevice( object sender, EventArgs e ) {
      _viewRegs.Clear();
      _registers.Clear();
      _clnEllipseFittings.Clear();
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

          _countTimes += _readWriteConvert;
          _dTime = TimeSpan.FromMilliseconds(_countTimes);

          SetColorEllipses( result[ 9 ], result[ 10 ] );
          SetPointsSeries( result[ 0 ], 0, _volt );
          SetPointsSeries( result[ 1 ], 1, _curr );
          SetPointsSeries( result[ 4 ], 4, _torq );
          SetPointsSeries( result[ 2 ], 2, _external );
          SetPointsSeries( result[ 3 ], 3, _motor );

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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_queryRegisters">Значение кнопки</param>
    /// <returns></returns>
    private bool _clearBtn;
    internal (ObservableCollection<string>, string, string, ObservableCollection<Ellipse>, ObservableCollection<ChartPoints>[], string, bool) RegistersRequest() {
      try {
        if( _serial.IsOpen ) {
          #region <Timer>
          _timer.Tick += new EventHandler( GetRegisterToDevice );
          _timer.Interval = new TimeSpan( 0, 0, 0, 0, Convert.ToInt32( _readWriteConvert ) );
          var listArrSeries = new ObservableCollection<ChartPoints>[] { _volt, _curr, _torq, _external, _motor };
          for( int i = 0; i < _arrSerires.Length; i++ ) {
            _arrSerires[ i ] = listArrSeries[ i ];
          }
          foreach( var series in _arrSerires ) {
            if( series == null )
              _cleanSeries = "Пусто";
            else
              _cleanSeries = "Очистить";
          }
          if( !_timer.IsEnabled ) {
            _timer.Start();
            _queryRegisters = "Stop";
            _errMessage = "Запущено...";
            _clearBtn = false;
          }
          else {
            _timer.Stop();
            _queryRegisters = "Start";
            _errMessage = "Остановлено...";
            _clearBtn = true;
            SetDbLinePoints();
          }
          #endregion
        }
      }
      catch( Exception err ) {
        _errMessage = err.Message.ToString();
      }
      return (_registers, _queryRegisters, _errMessage, _clnEllipseFittings, _arrSerires, _cleanSeries, _clearBtn);
    }
    internal (string, int) ConvertToInt( string _readWrite ) {
      if( _readWrite != "" ) {
        _readWriteConvert = (int)Convert.ToDouble( _readWrite );
        if( _readWriteConvert < _baseMinValReadWrite ) {
          _readWriteConvert = _baseMinValReadWrite;
          _errMessage = string.Format( "Интервал не может быть меньше {0} ms, поэтому задан интервал по умолчанию {0} ms.", _readWriteConvert );
        }
        else if( _readWriteConvert > _baseMaxValReadWrite ) {
          _readWriteConvert = _baseMaxValReadWrite;
          _errMessage = string.Format( "Значение не может превышать значение в {0} ms, поэтому задано значение по умолчанию {0} ms.", _readWriteConvert );
        }
        else {
          _errMessage = string.Format( "Значение интервала опроса устроства: {0} ms", _readWriteConvert );
        }
      }
      else {
        _errMessage = string.Format( "Задайте значение опроса устройства!" );
      }
      return (_errMessage, _readWriteConvert);
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
    private void SetPointsSeries( ushort _valRegister, int indexRegistrs, ObservableCollection<ChartPoints> _lineSeries  ) {
      double _valReg = 0;
      switch( indexRegistrs ) {
        case 0:
        _valReg = ConverValuesFfromRegisters( _valRegister, 17, 1300, 0.0, 80.0 );
        linePoints.Add( new LinePoint { LineGroupId = 1, Time = _dTime, Values = _valReg } );
        break;
        case 1:
        _valReg = ConverValuesFfromRegisters( _valRegister, 5, 46, 0.0, 52.0 );
        linePoints.Add( new LinePoint { LineGroupId = 2, Time = _dTime, Values = _valReg } );
        break;
        case 4:
        _valReg = ConverValuesFfromRegisters( _valRegister, 45, 4046, -1000.0, 1000.0 );
        linePoints.Add( new LinePoint { LineGroupId = 3, Time = _dTime, Values = _valReg } );
        break;
        case 2:
        _valReg = Convert.ToDouble( _valRegister );
        linePoints.Add( new LinePoint { LineGroupId = 4, Time = _dTime, Values = _valReg } );
        break;
        case 3:
        _valReg = Convert.ToDouble( _valRegister );
        linePoints.Add( new LinePoint { LineGroupId = 5, Time = _dTime, Values = _valReg } );
        break;
      }
      _lineSeries.Add( new ChartPoints { XTime = _dTime, YValue = _valReg } );
    }
    private double ConverValuesFfromRegisters( ushort inVal, double inMin, double inMax, double outMin, double outMax ) {
      var x = Convert.ToDouble( inVal );
      var result = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
      return Math.Round( result, 2 );
    }
    internal void CleanSeriesWithChart() {

      foreach( var item in _arrSerires ) {
        if( item != null ) {
          item.Clear();
        }
      }
      _countTimes = 0;
    }
    private async void SetDbSeriesLine() {

      using( var context = new SampleContext() ) {
        await Task.Run( () => {
          linePointGroup.ForEach( s => context.LinesGroup.AddOrUpdate( p => p.NameLine, s ) );
          context.SaveChanges();
          #region <Comment>

          //  var result = context.LinesGroup.GroupJoin(
          //    context.LinePoints,
          //    lpgItem => lpgItem.Id,
          //    lpItem => lpItem.LineGroupId,
          //    ( group, line ) => new {
          //      LineName = group.NameLine,
          //      LineData = line.Select( res => new { res.Time, res.Values } )
          //    } );

          //  foreach( var grpItem in result ) {
          //    Console.WriteLine( $"\n-----Line Name: {grpItem.LineName}-----\n" );
          //    foreach( var lineItem in grpItem.LineData ) {
          //      Console.WriteLine( $"\tTime: {lineItem.Time} Value: {lineItem.Values}" );
          //    }
          //  }
          //}

          //var keyPress = Console.ReadKey().Key;

          //if( keyPress == ConsoleKey.F ) {
          //  DeletedFromTable();
          #endregion
        } );
      }

    }
    private async void SetDbLinePoints() {
      try {
        using( var context = new SampleContext() ) {
          await Task.Run( () => {
            context.LinePoints.AddRange( linePoints );
            context.SaveChanges();
          } );
        }
      }
      catch( Exception err ) {
        _errMessage = err.Message.ToString();
      }
    }
    internal async void DeletedFromTable() {
      try {
        linePoints.Clear();
        using( var context = new SampleContext() ) {
          await Task.Run( () => {
            foreach( var delItem in context.LinePoints ) {
              context.LinePoints.Remove( delItem );
            }
            context.SaveChanges();
          } );
        }
      }
      catch( Exception err ) {
        _errMessage = err.Message.ToString();
      }
    }

  }
}

