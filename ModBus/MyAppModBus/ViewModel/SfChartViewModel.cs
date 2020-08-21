using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using MyAppModBus.Commands;
using MyAppModBus.Controllers;
using MyAppModBus.Models;
using MyAppModBus.View;
using MyAppModBus.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace MyAppModBus.ViewModel {
  internal class SfChartViewModel : ViewModelBase {
    private ControllerBase ctr = null;
    private static ExportChart expToExlWin = null;
    private List<string> _portList;
    private int _readWriteTimeOut;
    private string _errMessage = "Подключитесь к COM порту...";
    private string _selectedItem;
    private bool _elemEnable = true;
    private bool _elemDisable = false;
    private string _elemVisible = Visibility.Hidden.ToString();
    private string _readWrite = "";
    private ObservableCollection<string> _registers;
    private string _queryRegisters = "Start";
    private string _stateSerialPort = "Подключить";
    private string _cleanSeries = "очистить";
    private ObservableCollection<Ellipse> _colorEndFittings;
    private bool _clearBtn;

    #region Variables
    private TimeSpan _minValueTimeExl = new TimeSpan();
    private TimeSpan _maxValueTimeExl = new TimeSpan();
    private List<string> _nameSeriesExl = new List<string>();
    private int _selectitem;



    //Переменные Чекбоксов
    #region Чекбоксы
    private bool _chVolt = false;
    private bool _chCurr = false;
    private bool _chTorq = false;
    private bool _chExt = false;
    private bool _chMot = false;
    private bool? _chAll = null;
    #endregion
    #endregion

    #region ForExcel
    private string _nameSeries = "Series Title";
    private string _nameExcelFile;
    private int _minValueTime = 1000;
    private int _maxValueTime = 8000;
    #endregion

    #region Коллекции объектов точек серий 

    private ObservableCollection<ChartPoints> _pointsSeriesVolt;
    private ObservableCollection<ChartPoints> _pointsSeriesCurr;
    private ObservableCollection<ChartPoints> _pointsSeriesTorq;
    private ObservableCollection<ChartPoints> _pointsSeriesExtern;
    private ObservableCollection<ChartPoints> _pointsSeriesMotor;
    #endregion

    #region Свойства

    #region ForExcel
    public string NameSeries { get => _nameSeries; set => _nameSeries = value; }
    public string NameExcelFile { get => _nameExcelFile; set => _nameExcelFile = value; }

    public int MinValueTime { get => _minValueTime; set => _minValueTime = value; }
    public int MaxValueTime { get => _maxValueTime; set => _maxValueTime = value; }
    #endregion
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
    public string CleanSeries { get => _cleanSeries; set => Set( ref _cleanSeries, value ); }
    public bool ClearBtn { get => _clearBtn; set => Set( ref _clearBtn, value ); }
    public TimeSpan MinValueTimeExl { get => _minValueTimeExl; set => Set( ref _minValueTimeExl, value ); }
    public TimeSpan MaxValueTimeExl { get => _maxValueTimeExl; set => Set( ref _maxValueTimeExl, value ); }
    public List<string> NameSeriesExl { get => _nameSeriesExl; set => Set( ref _nameSeriesExl, value ); }
    public int SelectItem { get => _selectitem; set => Set( ref _selectitem, value ); }

    #region Свойства CheckBoxes
    public bool CheckBoxVolt { get => _chVolt; set => SetRightFlag( ref _chVolt, value ); }
    public bool CheckBoxCurr { get => _chCurr; set => SetRightFlag( ref _chCurr, value ); }
    public bool CheckBoxTorq { get => _chTorq; set => SetRightFlag( ref _chTorq, value ); }
    public bool CheckBoxExt { get => _chExt; set => SetRightFlag( ref _chExt, value ); }
    public bool CheckBoxMot { get => _chMot; set => SetRightFlag( ref _chMot, value ); }
    public bool? CheckBoxAll {
      get => _chAll;
      set
      {
        if( _chAll != value ) {
          Set( ref _chAll, value );
          if( _chAll.HasValue ) {
            CheckBoxVolt = CheckBoxCurr = CheckBoxTorq = CheckBoxExt = CheckBoxMot = (bool)_chAll;
          }
        }
      }
    }
    #endregion

    #region Свойства видимости и активности элементов

    public bool ElemEnable {
      get => _elemEnable;
      set => Set( ref _elemEnable, value );
    }
    public bool ElemDisable {
      get => _elemDisable;
      set => Set( ref _elemDisable, value );
    }
    public string ElemVisible { get => _elemVisible; set => Set( ref _elemVisible, value ); }

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
      get => _pointsSeriesVolt; set => Set( ref _pointsSeriesVolt, value );
    }
    public ObservableCollection<ChartPoints> PointSeriesCurr {
      get => _pointsSeriesCurr; set => Set( ref _pointsSeriesCurr, value );
    }
    public ObservableCollection<ChartPoints> PointSeriesTorq {
      get => _pointsSeriesTorq; set => Set( ref _pointsSeriesTorq, value );
    }
    public ObservableCollection<ChartPoints> PointSeriesMotor {
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

      if( _readWrite != "" ) {

        var conToDeviceItem = ctr.ConnectToDevice( _selectedItem );
        ErrMessage = conToDeviceItem.Item1;
        StateSerialPort = conToDeviceItem.Item2;
        QueryRegistrs = conToDeviceItem.Item3;
        ElemEnable = ctr.SetElementEnable( _elemEnable );
        ElemDisable = ctr.SetElementDisable( _elemEnable );
        ClearBtn = conToDeviceItem.Item4;
        ElemVisible = conToDeviceItem.Item5;
      }
      else {
        ErrMessage = "Задайте значение опроса устройства!";
      }
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
      PointSeriesVolt = arrSeries[ 0 ];
      PointSeriesCurr = arrSeries[ 1 ];
      PointSeriesTorq = arrSeries[ 2 ];
      PointSeriesExternal = arrSeries[ 3 ];
      PointSeriesMotor = arrSeries[ 4 ];
      CleanSeries = regRequests.Item6;
      ClearBtn = regRequests.Item7;
    }
    #endregion

    #region Конвертировать в Целое число

    public ICommand ConverToInt {
      get;
    }
    private bool CanConverToIntExecute( object p ) => true;
    private void OnConverToIntExecuted( object p ) {
      var cnvToInt = ctr.ConvertToInt( _readWrite );
      ErrMessage = cnvToInt.Item1;
    }

    #endregion

    #region Запись в регистры

    public ICommand WriteToRegisters {
      get;
    }

    private bool CanWriteToRegistersExute( object p ) => true;
    private void OnWriteToRegistersExuted( object p ) {

      var wrtRegs = ctr.WriteValuesToRegisters( p );
      ErrMessage = wrtRegs;

    }
    #endregion

    #region Открытие окна эксорта в ексель
    public ICommand ExportToExlWindow { get; set; }
    private bool CanSetDbLinesExecute( object p ) => true;
    private void OnSetDbLinesExecuted( object p ) {
      expToExlWin = new ExportChart();
      expToExlWin.ShowDialog();
    }
    #endregion

    #region Очистка графика

    public ICommand CleaningChart { get; set; }
    private bool CanCleaningChartExecute( object p ) => true;

    private void OnCleaningChartExecuted( object p ) {
      ctr.CleanSeriesWithChart();
    }

    #endregion

    #region Экспорт серии в ексель
    public ICommand ExportDataToXLS { get; set; }
    private bool CanExportToXLSExecute( object p ) => true;
    private void OnExportToXLSExecuted( object p ) {
      var arrBoolVal = new bool[] { _chVolt, _chCurr, _chTorq, _chExt, _chMot };
      ctr.ExportDataToExcelAsync( MinValueTimeExl, MaxValueTimeExl, arrBoolVal, _nameSeriesExl, _allSeries, _allTime);
    }
    #endregion

    #endregion

    public SfChartViewModel() {
      #region Команды
      ConnectToDevice = new LambdaCommand( OnSelectItemCommandExecuted, CanSelectItemCommandExecute );
      GetRegistersValues = new LambdaCommand( OnGetRegistersValuesExecuted, CanGetRegistersValuesExute );
      ConverToInt = new LambdaCommand( OnConverToIntExecuted, CanConverToIntExecute );
      WriteToRegisters = new LambdaCommand( OnWriteToRegistersExuted, CanWriteToRegistersExute );
      CleaningChart = new LambdaCommand( OnCleaningChartExecuted, CanCleaningChartExecute );
      ExportToExlWindow = new LambdaCommand( OnSetDbLinesExecuted, CanSetDbLinesExecute );
      ExportDataToXLS = new LambdaCommand( OnExportToXLSExecuted, CanExportToXLSExecute );
      #endregion

      ctr = new ControllerBase();

      ctr.AddItemToComboBox( ref _portList );

      MinValueTimeExl = TimeSpan.FromMilliseconds( 0 );
      MaxValueTimeExl = TimeSpan.FromMilliseconds( 0 );

      var name = new string[] { "Напряжение", "Ток", "Момент", "Обороты", "Empty" };
      _nameSeriesExl.AddRange( name );

    }

    private bool SetRightFlag( ref bool field, bool value, [CallerMemberName] string propName = null ) {
      if( field != value ) {
        Set( ref field, value, propName );
        UpdateAll();
        return true;
      }
      return false;
    }
    protected void UpdateAll() {
      //  Don't call the All setter from here, because it has side effects.
      if( CheckBoxVolt && CheckBoxCurr && CheckBoxTorq && CheckBoxExt && CheckBoxMot ) {
        _chAll = true;
        OnPropertyChanged( nameof( CheckBoxAll ) );
      }
      else if( !CheckBoxVolt && !CheckBoxCurr && !CheckBoxTorq && !CheckBoxExt && !CheckBoxMot ) {
        _chAll = false;
        OnPropertyChanged( nameof( CheckBoxAll ) );
      }
      else if( CheckBoxAll.HasValue ) {
        _chAll = null;
        OnPropertyChanged( nameof( CheckBoxAll ) );
      }
    }

  }
}
