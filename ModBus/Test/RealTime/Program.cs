using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;


namespace RealTime {
  internal class Program {

    private static string _nameExcelFile;
    private static Random rand = new Random();
    private static TimeSpan _time = new TimeSpan();
    private static string _nameSeries = "Series Title";

    public static string NameSeries { get => _nameSeries; set => _nameSeries = value; }
    public static string NameExcelFile { get => _nameExcelFile; set => _nameExcelFile = value; }

    internal static void Main( string[] args ) {

      var key = Console.ReadKey();
      if( key.Key == ConsoleKey.F ) {
        SaveSeriesData();
      }
      Console.WriteLine( "\n-------Нажмите Enter-------" );
      Console.ReadKey();
    }

    private async static void SaveSeriesData() {

      Excel.Application exApp = new Excel.Application();
      exApp.Workbooks.Add();
      Excel.Worksheet wsh = (Excel.Worksheet)exApp.ActiveSheet;

      var arrNums = new int[ 60 ];

      for( int i = 1; i < 3; i++ ) {
        if( (i) % 2 != 0 ) {
          wsh.Cells[ 1, i ] = "Time";
        }
        else {
          wsh.Cells[ 1, i ] = NameSeries;
        }

      }

      int countTime = 0;
      await Task.Run( () => {
        for( int i = 2; i < arrNums.Length; i++ ) {
          for( int j = 1; j < 3; j++ ) {
            _time = TimeSpan.FromSeconds( countTime );
            if( j % 2 == 0 ) {
              wsh.Cells[ i, j ] = rand.Next( 458, 6848 );
            }
            else {
              wsh.Cells[ i, j ] = _time.ToString();
            }
          }
          countTime++;
        }
      } );


      Excel.ChartObjects _chartObjectsSeries = (Excel.ChartObjects)wsh.ChartObjects();

      Excel.ChartObject _chartObjectSeries = _chartObjectsSeries.Add( 150, 25, 1024, 450 );

      _chartObjectSeries.Chart.ChartWizard( wsh.Range[ "A1", $"B{arrNums.Length - 1}" ], Excel.XlChartType.xlLineMarkers, "Title" );
      exApp.Visible = true;
    }
  }
}
