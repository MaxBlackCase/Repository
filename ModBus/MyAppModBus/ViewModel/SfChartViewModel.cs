using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using MyAppModBus.Commands;
using MyAppModBus.Controllers;
using MyAppModBus.Models;
using MyAppModBus.View.Pages;
using MyAppModBus.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;

namespace MyAppModBus.ViewModel {
  internal class SfChartViewModel : ViewModelBase {
    private List<string> _portList;
    private int _readWriteTimeOut;
    private string _errMessage = "Подключитесь к COM порту...";
    private string _selectedItem;
    private bool _elemEnable = true;
    private bool _elemDisable = false;
    private string _elemVisible = "Visible";
    private string _elemHidden = "Hidden";
    private string _readWrite;
    private ObservableCollection<string> _registers;
    private string _queryRegisters = "Start";
    private string _stateSerialPort = "Подключить";
    private ObservableCollection<Ellipse> _colorEndFittings;

    private ObservableCollection<ChartPoints> _pointsSeriesVolt;
    private ObservableCollection<ChartPoints> _pointsSeriesCurr;
    private ObservableCollection<ChartPoints> _pointsSeriesTorq;
    private ObservableCollection<ChartPoints> _pointsSeriesExtern;
    private ObservableCollection<ChartPoints> _pointsSeriesMotor;

    /// <summary>
    /// Контроллер
    /// </summary>
    private ControllerBase ctr = null;

    #region Свойства
    public List<string> PortList {
      get => _portList;
      set => Set( ref _portList, value );
      }
    public string SelectedItem {

      get => _selectedItem;
      set => Set( ref _selectedItem, value );

      }
    public string ReadWrite {
      get => _readWrite; set => Set( ref _readWrite, value );
      }
    public int ReadWriteTimeOut {
      get => _readWriteTimeOut;
      set => Set( ref _readWriteTimeOut, value );
      }
    public string ErrMessage {
      get => _errMessage;
      set => Set( ref _errMessage, value );
      }
    public string StateSerialPort {
      get => _stateSerialPort; set => Set( ref _stateSerialPort, value );
    }

    #region Свойства видимости и активности элементов
    public bool ElemEnable {
      get => _elemEnable;
      set => Set( ref _elemEnable, value );
    }
    public bool ElemDisable {
      get => _elemDisable;
      set => Set( ref _elemDisable, value );
    }
    public string ElemVisible {
      get => _elemVisible; set => Set( ref _elemVisible, value );
    }
    public string ElemHidden {
      get => _elemHidden; set => Set( ref _elemHidden, value );
    }
    #endregion

    #region Свойства отрисовки концевиков
    public ObservableCollection<Ellipse> ColorEndFittings {
      get => _colorEndFittings; set => Set( ref _colorEndFittings, value );
    }
    public ObservableCollection<string> Registers {
      get => _registers; set => Set( ref _registers, value );
    }
    public string QueryRegistrs {
      get => _queryRegisters; set => Set( ref _queryRegisters, value );
    }
    #endregion

    #region Серии линий графика
    public ObservableCollection<ChartPoints> PointSeriesVolt {
      get => _pointsSeriesVolt; set => Set(ref _pointsSeriesVolt, value);
    }
    public ObservableCollection<ChartPoints> PointSeriesCurr {
      get => _pointsSeriesCurr; set => Set( ref _pointsSeriesCurr, value );
    }
    public ObservableCollection<ChartPoints> PointSeriesTorq {
      get => _pointsSeriesTorq; set => Set( ref _pointsSeriesTorq, value );
    }
    public ObservableCollection<ChartPoints> PointSeriesMotor{
      get => _pointsSeriesMotor; set => Set( ref _pointsSeriesMotor, value );
    }
    public ObservableCollection<ChartPoints> PointSeriesExternal {
      get => _pointsSeriesExtern; set => Set( ref _pointsSeriesExtern, value );
    }

    #endregion

    #endregion

    #region Команды

    #region Команда поключения/отключения к/от COM порта и отображение UI элементов
    public ICommand ConnectToDevice {
      get;
      }
    private bool CanSelectItemCommandExecute( object p ) => true;
    private void OnSelectItemCommandExecuted( object p ) {

      var conToDeviceItem = ctr.ConnectToDevice( _selectedItem );

      ErrMessage = conToDeviceItem.Item1;
      StateSerialPort = conToDeviceItem.Item2;
      QueryRegistrs = conToDeviceItem.Item3;
      ElemEnable = ctr.SetElementEnable( _elemEnable );
      ElemDisable = ctr.SetElementDisable( _elemEnable );

      }
    #endregion

    #region Запрос регистров
    public ICommand GetRegistersValues {
      get;
      }
    private bool CanGetRegistersValuesExute( object p ) => true;
    private void OnGetRegistersValuesExecuted( object p ) {
      var regRequests = ctr.RegistersRequest();
      var arrSeries = regRequests.Item5;
      Registers = regRequests.Item1;
      QueryRegistrs = regRequests.Item2;
      ErrMessage = regRequests.Item3;
      ColorEndFittings = regRequests.Item4;
      PointSeriesVolt = arrSeries[0];
      PointSeriesCurr = arrSeries[ 1 ];
      PointSeriesTorq = arrSeries[ 2 ];
      PointSeriesExternal = arrSeries[ 3 ];
      PointSeriesMotor = arrSeries[ 4 ];
    }
    #endregion

    #region Конвертировать в Целое число

    public ICommand ConverToInt {
      get;
      }
    private bool CanConverToIntExecute( object p ) => true;
    private void OnConverToIntExecuted( object p ) {
      ErrMessage = ctr.ConvertToInt( _readWrite );
      }

    #endregion

    #region Запись в регистры

    public ICommand WriteToRegisters {
      get;
      }

    private bool CanWriteToRegistersExute( object p ) => true;
    private void OnWriteToRegistersExuted( object p ) {

       var wrtRegs = ctr.WriteValuesToRegisters(p);
      ErrMessage = wrtRegs;

      }
    #endregion

    #region Открытие окна с данными о точках на графике
    public ICommand SetDbLines { get; set; }
    private bool CanSetDbLinesExecute( object p ) => true;
    private void OnSetDbLinesExecuted(object p ) {
      var winDbLines = new DbLines();
      winDbLines.Show();
    }
    #endregion

    #endregion
    public SfChartViewModel() {
      #region Команды
      ConnectToDevice = new LambdaCommand( OnSelectItemCommandExecuted, CanSelectItemCommandExecute );
      GetRegistersValues = new LambdaCommand( OnGetRegistersValuesExecuted, CanGetRegistersValuesExute );
      ConverToInt = new LambdaCommand( OnConverToIntExecuted, CanConverToIntExecute );
      WriteToRegisters = new LambdaCommand( OnWriteToRegistersExuted, CanWriteToRegistersExute );
      SetDbLines = new LambdaCommand( OnSetDbLinesExecuted, CanSetDbLinesExecute );
      #endregion

      ctr = new ControllerBase();
      ctr.AddItemToComboBox( ref _portList );
      }
    }
  }
