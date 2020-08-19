using System;
using System.Collections.Generic;
using System.Windows.Input;
using MyAppModBus.Commands;
using MyAppModBus.Controllers;
using MyAppModBus.ViewModel.Base;

namespace MyAppModBus.ViewModel {
  internal class ExportChartViewModel : ViewModelBase {

    private TimeSpan _minValueTime = new TimeSpan();
    private TimeSpan _maxValueTime = new TimeSpan();
    private List<string> _nameSeries = new List<string>();
    private int _selectitem;

    /// Класс контроллера
    private ControllerBase _ctr = new ControllerBase();

    #region Свойства
    public TimeSpan MinValueTime { get => _minValueTime; set => Set( ref _minValueTime, value ); }
    public TimeSpan MaxValueTime { get => _maxValueTime; set => Set( ref _maxValueTime, value ); }

    public List<string> NameSeries { get => _nameSeries; set => Set( ref _nameSeries, value ); }
    public int SelectItem { get => _selectitem; set => Set( ref _selectitem, value ); }
    #endregion

    #region Команды
    #region Экспорт серии в ексель
    public ICommand ExportToXLS { get; set; }
    private bool CanExportToXLSExecute( object p ) => true;
    private void OnExportToXLSExecuted( object p ) {
      _ctr.ExportDataToExcel( MinValueTime, MaxValueTime, SelectItem );
    }
    #endregion
    #endregion

    public ExportChartViewModel() {

      #region Команды
      ExportToXLS = new LambdaCommand( OnExportToXLSExecuted, CanExportToXLSExecute );
      #endregion

      MinValueTime = TimeSpan.FromMilliseconds( 0 );
      MaxValueTime = TimeSpan.FromMilliseconds( 0 );
      var name = new string[] { "Напряжение", "Ток", "Момент", "Обороты", "Test" };

      _nameSeries.AddRange( name );

    }

  }
}
