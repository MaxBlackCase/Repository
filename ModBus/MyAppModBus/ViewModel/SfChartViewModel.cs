﻿using ControlzEx.Standard;
using InteractiveDataDisplay.WPF;
using MyAppModBus.Commands;
using MyAppModBus.Controllers;
using MyAppModBus.Models;
using MyAppModBus.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
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
    private List<ChartPoints> _pointsSeries;
    private ControllerBase ctr = null;

    #region Свойства

    /// <summary>
    /// Список портов
    /// </summary>
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
    public string StateSerialPort {
      get => _stateSerialPort; set => Set( ref _stateSerialPort, value );
      }
    public ObservableCollection<Ellipse> ColorEndFittings {
      get => _colorEndFittings; set => Set( ref _colorEndFittings, value );
      }
    public ObservableCollection<string> Registers {
      get => _registers; set => Set( ref _registers, value );
      }
    public string QueryRegistrs {
      get => _queryRegisters; set => Set( ref _queryRegisters, value );
      }

    public List<ChartPoints> PointsSeries {
      get => _pointsSeries; set => Set(ref _pointsSeries, value);
    }

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

      Registers = regRequests.Item1;
      QueryRegistrs = regRequests.Item2;
      ErrMessage = regRequests.Item3;
      ColorEndFittings = regRequests.Item4;
      PointsSeries = regRequests.Item5;

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

    #endregion

    public SfChartViewModel() {
      #region Команды
      ConnectToDevice = new LambdaCommand( OnSelectItemCommandExecuted, CanSelectItemCommandExecute );
      GetRegistersValues = new LambdaCommand( OnGetRegistersValuesExecuted, CanGetRegistersValuesExute );
      ConverToInt = new LambdaCommand( OnConverToIntExecuted, CanConverToIntExecute );
      WriteToRegisters = new LambdaCommand( OnWriteToRegistersExuted, CanWriteToRegistersExute );
      #endregion

      ctr = new ControllerBase();
      ctr.AddItemToComboBox( ref _portList );
      }
    }
  }
