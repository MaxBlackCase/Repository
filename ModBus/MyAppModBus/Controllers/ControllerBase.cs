using MahApps.Metro.Controls;
using Modbus.Device;
using MyAppModBus.Models;
using MyAppModBus.ViewModel;
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

      for (var i = 0; i < ports.Length; i++) {
        if (ports[ i ] != null) {
          string str = ports[ i ].ToString();
          int maxLength = 3;
          string result = str.Substring( 0, Math.Min( str.Length, maxLength ) );
          if (result == "COM") {
            _portList.Add( ports[ i ] );
            }
          } else {
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
        if (_serial.IsOpen) {
          DisconnectToDevice();
          _errMessage = string.Format( "{0} порт Отключен", _portName );
          _stateSerialPort = "Подключить";
          } else {
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
        }
      catch (Exception err) {
        _errMessage = err.Message.ToString();
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
      _serial.Close();
      _serial.Dispose();
      _timer.Stop();
      _queryRegisters = "Start";
      }

    #region Elements IsEnable, IsVisibility
    /// <summary>
    /// Переключатель активности для елементов до подключения к устройству
    /// </summary>
    /// <param name="_elemEnable">Значение переключателя</param>
    /// <returns></returns>
    internal bool SetElementEnable( bool _elemEnable ) {
      if (_elemEnable == true && _serial.IsOpen) {
        _elemEnable = false;
        } else {
        _elemEnable = true;
        }
      return _elemEnable;
      }
    internal bool SetElementDisable( bool _elemDisable ) {
      if (_elemDisable == true && _serial.IsOpen) {
        _elemDisable = false;
        } else {
        _elemDisable = true;
        }
      return _elemDisable;
      }
    /// <summary>
    /// Переключатель отображения для елементов до подключения к устройству
    /// </summary>
    /// <param name="_elemEnable">Значение переключателя</param>
    /// <returns></returns>
    internal string SetElementVisible( string _elemVis ) {
      if (_elemVis == Visibility.Visible.ToString() && _serial.IsOpen) {
        _elemVis = Visibility.Hidden.ToString();
        } else {
        _elemVis = Visibility.Visible.ToString();
        }
      return _elemVis;
      }
    internal string SetElementHidden( string _elemHid ) {
      if (_elemHid == Visibility.Hidden.ToString() && _serial.IsOpen) {
        _elemHid = Visibility.Visible.ToString();
        } else {
        _elemHid = Visibility.Hidden.ToString();
        }
      return _elemHid;
      }
    #endregion


    internal double countTime = 0;
    internal double countIndex = 0;
    private int[][] _numberRegisters = new int[ 2 ][];


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
        if (_serial.IsOpen) {
          ushort[] result = _master.ReadHoldingRegisters( slaveID, startAddress, numburOfPoints );
          for (int i = 0; i < result.Length; i++) {
            _viewRegs.Add( new ViewRegisterModel { ID = i, Value = result[ i ].ToString() } );
            }
          foreach (var item in _viewRegs) {
            _registers.Add( $"Регистр: {item.ID}\t|  {item.Value}" );
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

          SetColorEllipses( result[ 9 ], result[ 10 ] );

          countIndex++;
          } else {
          _timer.Stop();
          }
        }
      catch (Exception err) {
        _registers.Add( err.Message.ToString() );
        _timer.Stop();
        }

      }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_queryRegisters">Значение кнопки</param>
    /// <returns></returns>
    internal (ObservableCollection<string>, string, string, ObservableCollection<Ellipse>) RegistersRequest() {
      if (!_timer.IsEnabled && _serial.IsOpen) {
        #region <Timer>
        _timer.Tick += new EventHandler( GetRegisterToDevice );
        _timer.Interval = new TimeSpan( 0, 0, 0, 0, Convert.ToInt32( _readWriteConvert ) );
        _timer.Start();
        _queryRegisters = "Stop";
        _errMessage = "Запущено...";
        #endregion
        } else {
        _timer.Stop();
        _errMessage = "Остановлено";
        _queryRegisters = "Start";
        }
      return (_registers, _queryRegisters, _errMessage, _clnEllipseFittings);
      }
    internal string ConvertToInt( string _readWrite ) {

      if (_readWrite != null) {
        _readWriteConvert = Convert.ToInt32( _readWrite );
        if (_readWriteConvert < 20) {
          _readWriteConvert = 20;
          _errMessage = string.Format( "Интервал не может быть меньше {0} ms, поэтому задан интервал по умолчанию {0} ms.", _readWriteConvert );
          } else if (_readWriteConvert > 100) {
          _readWriteConvert = 100;
          _errMessage = string.Format( "Значение не может превышать значение в {0} ms, поэтому задано значение по умолчанию {0} ms.", _readWriteConvert );
          } else {
          _errMessage = string.Format( "Значение интервала опроса устроства: {0} ms", _readWriteConvert );
          }
        }
      return _errMessage;
      }
    private void SetColorEllipses( ushort valOne, ushort valTwo ) {
      var regFit = new ushort[ 2 ];
      regFit[ 0 ] = valOne;
      regFit[ 1 ] = valTwo;

      for (int i = 0; i < regFit.Length; i++) {
        if (regFit[ i ] == 1) {
          _ellipseFittings = new Ellipse {
            Width = 25,
            Height = 25,
            Fill = Brushes.Green,
            Margin = new Thickness( 5 )
            };
          } else {
          _ellipseFittings = new Ellipse {
            Width = 25,
            Height = 25,
            Fill = Brushes.Red,
            Margin = new Thickness( 5 )
            };
          }
        _clnEllipseFittings.Add( _ellipseFittings );
        }
      //if (valTwo == 1) {
      //  _corEndFittings.Item2 = Colors.Green.ToString();
      //  } else {
      //  _corEndFittings.Item2 = Colors.Red.ToString();
      //  }
      }


    internal void WriteValuesToRegisters() {
      //ToggleSwitch toggleSwitch = sender as ToggleSwitch;
      if (toggleSwitch != null) {
        if (toggleSwitch.IsOn == true) {
          togElem.IsActive = true;
          progress.Visibility = Visibility.Visible;
          } else {
          progress.IsActive = false;
          progress.Visibility = Visibility.Collapsed;
          }
        }
      }

    //private void GetSfCharts() {

    //  Data = new List<DataPoints>() {

    //    new DataPoints {Name = "David", Height = 180},
    //    new DataPoints {Name = "Max", Height = 150},
    //    new DataPoints {Name = "Ulya", Height = 178},
    //    new DataPoints {Name = "Brad", Height = 162},

    //  };

    //  SfChart ch = new SfChart();

    //  CategoryAxis primaryAxis = new CategoryAxis();

    //  primaryAxis.Header = @"/Name/";

    //  ch.PrimaryAxis = primaryAxis;

    //  NumericalAxis seceondoryAxis = new NumericalAxis();
    //  seceondoryAxis.Header = "Height(in cm)";
    //  ch.SecondaryAxis = seceondoryAxis;

    //  ColumnSeries sers = new ColumnSeries();

    //  sers.ItemsSource = this.Data;
    //  sers.XBindingPath = "Name";
    //  sers.YBindingPath = "Height";

    //  ch.Series.Add( sers );
    //  }

    }
  }

