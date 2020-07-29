using MyAppModBus.Controllers;
using MyAppModBus.ViewModel.Base;
using System.Collections.Generic;

namespace MyAppModBus.ViewModel
{

  internal class SfChartViewModel : ViewModelBase
  {

    //private ControllerBase ctr = new ControllerBase();
    private List<string> _portList = new List<string>();
    private int _readWriteTimeOut;
    private string _errMessage;
    private string _visibilityButton;
    #region Свойства

    /// <summary>
    /// Список портов
    /// </summary>
    public List<string> PortList
    {
      get => _portList;
      set => Set(ref _portList, value);
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

    public string VisibilityButton
    {
      get => _visibilityButton;
      set => Set(ref _visibilityButton, value);
    }

    #endregion

    public SfChartViewModel()
    {

      ControllerBase ctr = new ControllerBase();

      ctr.AddItemToComboBox(PortList);
    }

  }
}
