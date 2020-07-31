using MyAppModBus.Commands;
using MyAppModBus.Controllers;
using MyAppModBus.Models;
using MyAppModBus.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace MyAppModBus.ViewModel
{
   internal class SfChartViewModel : ViewModelBase
   {
      private DispatcherTimer _timer;
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
      private string _queryRegisters = "Off";
      private string _pullTimeText;

      private ObservableCollection<ViewRegisterModel> _viewRegisterModels;

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
      public string SelectedItem
      {

         get => _selectedItem;
         set => Set(ref _selectedItem, value);

      }

      public string ReadWrite { get => _readWrite; set => Set(ref _readWrite, value); }

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

      public ObservableCollection<string> Registers { get => _registers; set => Set(ref _registers, value); }

      public string QueryRegistrs { get => _queryRegisters; set => Set(ref _queryRegisters, value); }

      public string PullTimeText { get => _pullTimeText; set => Set(ref _pullTimeText, value); }

      #endregion

      #region Команды

      #region Команда поключения к COM порту и отображение UI элементов
      public ICommand ConnectToDevice { get; }
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

      #region Запрос регистров
      public ICommand GetRegistersValues { get; }
      private bool CanGetRegistersValuesExute(object p) => true;
      private void OnGetRegistersValuesExecuted(object p)
      {
         QueryRegistrs = ctr.RegistersRequest(_queryRegisters);
      }
      #endregion

      #region Конвертировать в Целое число

      public ICommand ConverToInt { get; }
      private bool CanConverToIntExecute(object p) => true;
      private void OnConverToIntExecuted(object p)
      {
         ErrMessage = ctr.ConvertToInt(_readWrite, _errMessage);
      }

      #endregion

      #region Проверга регулярным выражением строки опроса устройства

      public ICommand TextSelectedRegular { get; }

      private bool CanTextSelectedExecute(object p) => true;

      private void OnTextSelectedExecuted(object p)
      {
         ReadWrite = ctr.CheckReadWriteOnRegular(_readWrite);
      }

      #endregion

      #endregion

      public SfChartViewModel()
      {
         #region Команды
         ConnectToDevice = new LambdaCommand(OnSelectItemCommandExecuted, CanSelectItemCommandExecute);
         DisconnectToDevice = new LambdaCommand(OnDisconnectToDeviceExecuted, CanDisconnectToDeviceExecute);
         GetRegistersValues = new LambdaCommand(OnGetRegistersValuesExecuted, CanGetRegistersValuesExute);
         ConverToInt = new LambdaCommand(OnConverToIntExecuted, CanConverToIntExecute);
         TextSelectedRegular = new LambdaCommand(OnTextSelectedExecuted, CanTextSelectedExecute);
         #endregion

         ctr = new ControllerBase();
         ctr.AddItemToComboBox(ref _portList);
      }
   }
}
