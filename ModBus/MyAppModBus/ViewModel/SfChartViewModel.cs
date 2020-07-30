using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using MyAppModBus.Commands;
using MyAppModBus.Controllers;
using MyAppModBus.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyAppModBus.ViewModel
{
  internal class SfChartViewModel : ViewModelBase
  {
    private List<string> _portList;
    private int _readWriteTimeOut;
    private string _errMessage = "Подключитесь к COM порту...";
    private string _selectedItem;
    private bool _elemEnable = true;
    private bool _elemDisable = false;
    private string _elemVisible =  "Visible";
    private string _elemHidden = "Hidden";

    private ControllerBase ctr = null;

    #region Свойства

    /// <summary>
    /// Список портов
    /// </summary>
    public List<string> PortList
    {
      get => _portList;
      set => Set(ref _portList, value);
    }
    public string SelectedItem {

      get => _selectedItem;
      set => Set(ref _selectedItem, value);

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

    public bool ElemEnable
    {
      get => _elemEnable;
      set => Set(ref _elemEnable, value);
    }
    public bool ElemDisable
    {
      get => _elemDisable;
      set => Set(ref _elemDisable, value);
    }

    public string ElemVisible { get => _elemVisible; set => Set(ref _elemVisible, value); }
    public string ElemHidden { get => _elemHidden; set => Set(ref _elemHidden, value); }

    #endregion

    #region Команды

    #region Команда поключения к COM порту и отображение UI элементов
    public ICommand ConnectToDevice{ get; }
    private bool CanSelectItemCommandExecute(object p) => true;
    private void OnSelectItemCommandExecuted(object p)
    {

      ErrMessage = ctr.ConnectToDevice(_selectedItem, _errMessage);
      ElemEnable = ctr.SetElementEnable(_elemEnable);
      ElemDisable = ctr.SetElementDisable(_elemEnable);
      ElemVisible = ctr.SetElementVisible(_elemVisible);
      ElemHidden = ctr.SetElementHidden(_elemHidden);

    }
    #endregion

    #region Команда отключения от COM порта

    public ICommand DisconnectToDevice { get; }

    private bool CanDisconnectToDeviceExecute(object p) => true;
    private void OnDisconnectToDeviceExecuted(object p)
    {
      ErrMessage = ctr.DisconnectToDevice(_selectedItem, _errMessage);
      ElemEnable = ctr.SetElementEnable(_elemEnable);
      ElemDisable = ctr.SetElementDisable(_elemEnable);
      ElemVisible = ctr.SetElementVisible(_elemVisible);
      ElemHidden = ctr.SetElementHidden(_elemHidden);
    }

    #endregion

    #endregion

    public SfChartViewModel()
    {
      #region Команды
      ConnectToDevice = new LambdaCommand(OnSelectItemCommandExecuted, CanSelectItemCommandExecute);
      DisconnectToDevice = new LambdaCommand(OnDisconnectToDeviceExecuted, CanDisconnectToDeviceExecute);
      #endregion

      ctr = new ControllerBase();
      ctr.AddItemToComboBox(ref _portList);
    }
  }
}
