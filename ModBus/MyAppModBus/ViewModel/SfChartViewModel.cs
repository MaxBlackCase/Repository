using MyAppModBus.ViewModel.Base;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyAppModBus.ViewModel
{
  internal class SfChartViewModel : ViewModelBase
  {

    private int _Time = 20;

    public int Interval { get => _Time; set => Set(ref _Time, value); }

  }
}
