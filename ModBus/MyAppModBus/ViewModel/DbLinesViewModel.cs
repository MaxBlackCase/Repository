using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyAppModBus.Models.DbModel;
using MyAppModBus.ViewModel.Base;

namespace MyAppModBus.ViewModel {
  internal class DbLinesViewModel : ViewModelBase {

    private Random rand = new Random();

    private ObservableCollection<LinePoint> _orders;
    public ObservableCollection<LinePoint> Orders { get => _orders; set => Set( ref _orders, value ); }

    public DbLinesViewModel() {
      _orders = new ObservableCollection<LinePoint>();
      this.GenerateOrders();
    }

    private async void GenerateOrders() {
      await Task.Run( () => {
        for( int i = 0; i < 20; i++ ) {
          _orders.Add( new LinePoint { 
            Id = i, 
            LineGroupId = rand.Next(1, 6), 
            Time = TimeSpan.FromSeconds(rand.Next(20, 60)),
            Values = rand.Next(1564, 87984)
          } );
        }
      } );
    }
  }
}
