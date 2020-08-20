using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MyAppModBus.Commands;
using MyAppModBus.Controllers;
using MyAppModBus.ViewModel.Base;

namespace MyAppModBus.ViewModel {
  internal class ExportChartViewModel : ViewModelBase {

    #region Variables
    private TimeSpan _minValueTime = new TimeSpan();
    private TimeSpan _maxValueTime = new TimeSpan();
    private List<string> _nameSeries = new List<string>();
    private int _selectitem;

    /// Класс контроллера
    private ControllerBase _ctr = new ControllerBase();

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

    #region Свойства

    public TimeSpan MinValueTime { get => _minValueTime; set => Set( ref _minValueTime, value ); }
    public TimeSpan MaxValueTime { get => _maxValueTime; set => Set( ref _maxValueTime, value ); }
    public List<string> NameSeries { get => _nameSeries; set => Set( ref _nameSeries, value ); }
    public int SelectItem { get => _selectitem; set => Set( ref _selectitem, value ); }

    #region Свойства CheckBoxes
    public bool CheckBoxVolt { get => _chVolt; set => SetRightFlag(ref _chVolt, value); }
    public bool CheckBoxCurr { get => _chCurr; set => SetRightFlag( ref _chCurr, value ); }
    public bool CheckBoxTorq { get => _chTorq; set => SetRightFlag(ref _chTorq, value); }
    public bool CheckBoxExt { get => _chExt; set => SetRightFlag(ref _chExt, value); }
    public bool CheckBoxMot { get => _chMot; set => SetRightFlag(ref _chMot, value); }
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

    #endregion

    #region Команды
    #region Экспорт серии в ексель
    public ICommand ExportToXLS { get; set; }
    private bool CanExportToXLSExecute( object p ) => true;
    private void OnExportToXLSExecuted( object p ) {
      var arrBoolVal = new bool[] { _chVolt, _chCurr, _chTorq, _chExt, _chMot };
      _ctr.ExportDataToExcelAsync( MinValueTime, MaxValueTime, arrBoolVal, _nameSeries);
    }
    #endregion
    #endregion

    public ExportChartViewModel() {

      #region Команды
      ExportToXLS = new LambdaCommand( OnExportToXLSExecuted, CanExportToXLSExecute );
      #endregion

      MinValueTime = TimeSpan.FromMilliseconds( 0 );
      MaxValueTime = TimeSpan.FromMilliseconds( 0 );

      var name = new string[] { "Напряжение", "Ток", "Момент", "Обороты", "Empty" };
      _nameSeries.AddRange( name );

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
