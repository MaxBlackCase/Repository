using Modbus.Device;
using MyAppModBus.Controllers;
using MyAppModBus.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Documents;
using System.Windows.Forms;
using TestDataGenerator;

namespace MyAppModBus.ViewModel
{

  internal class SfChartViewModel : ViewModelBase
  {

    private List<string> _portList = new List<string>();

    private ControllerBase ctr = new ControllerBase();

    #region Свойства

    /// <summary>
    /// Список портов
    /// </summary>
    public List<string> PortList
    {
      get => _portList;
      set => Set(ref _portList, value);
    }

    #endregion

    public SfChartViewModel()
    {
      ctr.AddItemToComboBox(PortList);
    }

    //private int countIndex = 0;
    //private void GetHoldReg()
    //{
    //  ushort[] result = _master.ReadHoldingRegisters(slaveID, startAddress, numburOfPoints);
    //  try
    //  {
    //    ///Занесение значений на график
    //    if (_countTime % _readWrite == 0)
    //    {
    //      #region OldGraphs
    //      //for ( int valueFirstChart = 0; valueFirstChart < _arrDict[ 0 ].Count(); valueFirstChart++ ) {
    //      //  _arrDict[ 0 ][ valueFirstChart ].Add( countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 0 ][ valueFirstChart ] ] ) );
    //      //  //Очищение коллекции точек График 1
    //      //  if (_arrDict[0][valueFirstChart].Count > 1000) { _arrDict[0][valueFirstChart].Clear(); }
    //      //}
    //      //for ( int valueSecondChart = 0; valueSecondChart < _arrDict[ 1 ].Count(); valueSecondChart++ ) {
    //      //  _arrDict[ 1 ][ valueSecondChart ].Add(countTime / 1000, Convert.ToDouble( result[ _numberRegisters[ 1 ][ valueSecondChart ] ] ) );
    //      //  //Очищение коллекции точек График 2
    //      //  if (_arrDict[1][valueSecondChart].Count > 1000){_arrDict[1][valueSecondChart].Clear(); }
    //      //}

    //      //for ( int valueFirstChart = 0; valueFirstChart < _linesArr[ 0 ].Length; valueFirstChart++ ) {
    //      //  _linesArr[ 0 ][ valueFirstChart ].Plot( _arrDict[ 0 ][ valueFirstChart ].Keys, _arrDict[ 0 ][ valueFirstChart ].Values );
    //      //}
    //      //for ( int valueSecondChart = 0; valueSecondChart < _linesArr[ 1 ].Length; valueSecondChart++ ) {
    //      //  _linesArr[ 1 ][ valueSecondChart ].Plot( _arrDict[ 1 ][ valueSecondChart ].Keys, _arrDict[ 1 ][ valueSecondChart ].Values );
    //      //}
    //      #endregion

    //      #region NewGraphs

    //      #endregion

    //    }

    //    countIndex++;
    //  }
    //  catch (Exception err)
    //  {
    //    MessageBox.Show(err.Message);
    //  }

    //}

  }
}
