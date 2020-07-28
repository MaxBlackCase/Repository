using Modbus.Device;
using MyAppModBus.ViewModel.Base;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace MyAppModBus.ViewModel
{

  internal class SfChartViewModel : ViewModelBase
  {

    const byte slaveID = 1;

    private readonly ushort startAddress = 0;
    private readonly ushort numburOfPoints = 18;
    private string _text;
    private double _countTime;
    private SerialPort _serial;
    private ModbusSerialMaster _master;
    private int _readWrite;

    public double CountTime { get => _countTime; set => Set(ref _countTime, value); }
    public SerialPort Serial { get => _serial; set => Set(ref _serial, value); }
    public ModbusSerialMaster Master { get => _master; set => Set(ref _master, value); }
    public int ReadWrite { get => _readWrite; set => Set(ref _readWrite, value); }
    public string Text { get => _text; set => Set(ref _text, value); }

    public SfChartViewModel()
    {
      
    }

    private int countIndex = 0;
    private void GetHoldReg()
    {
      ushort[] result = Master.ReadHoldingRegisters(slaveID, startAddress, numburOfPoints);
      try
      {
        ///Занесение значений на график
        if (CountTime % ReadWrite == 0)
        {
          #region OldGraphs
          //for ( int valueFirstChart = 0; valueFirstChart < _arrDict[ 0 ].Count(); valueFirstChart++ ) {
          //  _arrDict[ 0 ][ valueFirstChart ].Add( countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 0 ][ valueFirstChart ] ] ) );
          //  //Очищение коллекции точек График 1
          //  if (_arrDict[0][valueFirstChart].Count > 1000) { _arrDict[0][valueFirstChart].Clear(); }
          //}
          //for ( int valueSecondChart = 0; valueSecondChart < _arrDict[ 1 ].Count(); valueSecondChart++ ) {
          //  _arrDict[ 1 ][ valueSecondChart ].Add(countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 1 ][ valueSecondChart ] ] ) );
          //  //Очищение коллекции точек График 2
          //  if (_arrDict[1][valueSecondChart].Count > 1000){_arrDict[1][valueSecondChart].Clear(); }
          //}

          //for ( int valueFirstChart = 0; valueFirstChart < _linesArr[ 0 ].Length; valueFirstChart++ ) {
          //  _linesArr[ 0 ][ valueFirstChart ].Plot( _arrDict[ 0 ][ valueFirstChart ].Keys, _arrDict[ 0 ][ valueFirstChart ].Values );
          //}
          //for ( int valueSecondChart = 0; valueSecondChart < _linesArr[ 1 ].Length; valueSecondChart++ ) {
          //  _linesArr[ 1 ][ valueSecondChart ].Plot( _arrDict[ 1 ][ valueSecondChart ].Keys, _arrDict[ 1 ][ valueSecondChart ].Values );
          //}
          #endregion

          #region NewGraphs

          #endregion

        }

        countIndex++;
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }

    }

  }
}
