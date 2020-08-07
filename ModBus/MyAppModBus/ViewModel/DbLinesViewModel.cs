using MyAppModBus.Models.DbModel;
using MyAppModBus.ViewModel.Base;
using System;
using System.Collections.Generic;

namespace MyAppModBus.ViewModel {
  internal class DbLinesViewModel : ViewModelBase {

    private List<int> _dbLines;
    private int _IdLine;
    private TimeSpan _timeLine;
    private double _valueLine;

    #region Свойства

    public List<int> DbLines { get => _dbLines; set => Set( ref _dbLines, value ); }

    public int IdLine { get => _IdLine; set => Set( ref _IdLine, value ); }
    public TimeSpan TimeLine { get => _timeLine; set => Set( ref _timeLine, value ); }
    public double ValueLine { get => _valueLine; set => Set( ref _valueLine, value ); }
    #endregion

    public DbLinesViewModel() {

      DbLines = new List<int>();
      var rand = new Random();

      for( int i = 0; i < 10; i++ ) {
        DbLines.Add( rand.Next( 0, 452 ) );
      }

    }

  }
}
