using MyAppModBus.Models.DbModel;
using MyAppModBus.ViewModel.Base;
using System;
using System.Collections.Generic;

namespace MyAppModBus.ViewModel {
  internal class DbLinesViewModel : ViewModelBase {

    private List<LineModel> _dbLines;
    private int _IdLine;
    private TimeSpan _timeLine;
    private double _valueLine;

    #region Свойства

    public List<LineModel> DbLines { get => _dbLines; set => Set( ref _dbLines, value ); }

    public int IdLine { get => _IdLine; set => Set( ref _IdLine, value ); }
    public TimeSpan TimeLine { get => _timeLine; set => Set( ref _timeLine, value ); }
    public double ValueLine { get => _valueLine; set => Set( ref _valueLine, value ); }
    #endregion

    public DbLinesViewModel() {

      DbLines = new List<LineModel>() {
                new LineModel{ Id = 1, Time = TimeSpan.FromMinutes(84848), LineValue = 152 }
            };

      IdLine = 1;
      TimeLine = TimeSpan.FromMinutes( 84848 );
      ValueLine = 154;

    }

  }
}
