using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;


namespace RealTime {
  internal class Program {

    private static string _nameExcelFile;
    private static Random rand = new Random();
    private static TimeSpan _time;
    private static string _nameSeries = "Series Title";
    private static Excel.Application exApp = new Excel.Application();
    private static string _exlPath = Environment.CurrentDirectory.ToString() + @"\ExcelFiles\" + NameSeries.Replace( " ", "" ) + @"_" + Path.GetRandomFileName().ToString() + @".xls";
    private static int _minValueTime = 1000;
    private static int _maxValueTime = 8000;

    private static List<string> _timeList = new List<string>();
    private static List<int> _valueList = new List<int>();

    public static string NameSeries { get => _nameSeries; set => _nameSeries = value; }
    public static string NameExcelFile { get => _nameExcelFile; set => _nameExcelFile = value; }

    public int MinValueTime { get => _minValueTime; set => _minValueTime = value; }
    public int MaxValueTime { get => _maxValueTime; set => _maxValueTime = value; }

    internal static void Main( string[] args ) {

      var key = Console.ReadKey();
      if( key.Key == ConsoleKey.F ) {
        SaveSeriesData();
      }
      Console.WriteLine( "\n-------Нажмите Enter-------" );
      Console.ReadKey();
    }

    private async static void SaveSeriesData() {

      exApp.Workbooks.Add();
      Excel.Worksheet wsh = (Excel.Worksheet)exApp.ActiveSheet;

      var arrNums = new int[ 100 ];

      for( int i = 1; i < 3; i++ ) {
        if( (i) % 2 != 0 ) {
          wsh.Cells[ 1, i ] = "Time";
        }
        else {
          wsh.Cells[ 1, i ] = "Series Value";
        }
      }
      int countTime = 0;
      await Task.Run( () => {
        for( int i = 0; i < arrNums.Length; i++ ) {
          _time = new TimeSpan();
          _time = TimeSpan.FromMilliseconds( countTime );
          if( countTime >= _minValueTime && countTime <= _maxValueTime ) {
            _valueList.Add( rand.Next( 458, 6848 ) );
            _timeList.Add( _time.ToString() );
          }
          countTime += 100;
        }

        for( int i = 0; i < _valueList.Count; i++ ) {
          for( int j = 1; j < 3; j++ ) {
            if( j % 2 == 0 ) {
              wsh.Cells[ i + 2, j ] = _valueList[i];
            }
            else {
              wsh.Cells[ i + 2, j ] = _timeList[i];
            }
          }
        }

        Excel.ChartObjects _chartObjectsSeries = (Excel.ChartObjects)wsh.ChartObjects();

        Excel.ChartObject _chartObjectSeries = _chartObjectsSeries.Add( 150, 25, 1024, 450 );

        _chartObjectSeries.Chart.ChartWizard( wsh.Range[ "A1", $"B{_valueList.Count + 1}" ], Excel.XlChartType.xlLineMarkers );

        _chartObjectSeries.Chart.ChartStyle = 273;


        Directory.CreateDirectory( Environment.CurrentDirectory + @"\ExcelFiles" );


        exApp.ActiveWorkbook.SaveAs( _exlPath, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, false, false );

        exApp.ActiveWorkbook.Close();
        exApp.Quit();
        Process.Start( _exlPath );
      } );
    }
  }
}
