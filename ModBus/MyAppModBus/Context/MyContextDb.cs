using System.Data.Entity;
using MyAppModBus.Models.DbModel;

namespace MyAppModBus.Context {
  internal class MyContextDb : DbContext {
    public MyContextDb() : base( "DbConnectionString" ) { }

    public DbSet<LineModel>  Lines{ get; set; }

  }
}
