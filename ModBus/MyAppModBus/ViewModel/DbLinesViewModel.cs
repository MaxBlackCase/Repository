using System;
using System.Collections.Generic;
using MyAppModBus.ViewModel.Base;

namespace MyAppModBus.ViewModel {
  internal class DbLinesViewModel : ViewModelBase {

    private List<(TimeSpan, int)> _lst = new List<(TimeSpan, int)>();

    public List<(TimeSpan, int)> List { get => _lst; set => Set(ref _lst, value); }
    public DbLinesViewModel() {
      var rand = new Random();
      for( int i = 0; i < 400; i++ ) {
        _lst.Add((TimeSpan.FromSeconds(rand.Next(1000, 10000)), rand.Next(-1000, 9561)) );
      }

    }
  }
}
