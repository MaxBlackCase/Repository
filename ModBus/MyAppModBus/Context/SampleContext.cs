using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using MyAppModBus.Models.DbModel;

namespace MyAppModBus.Context {
  internal class SampleContext : DbContext {

    internal SampleContext() :base( "DbConnectionString" ) {}
    public DbSet<LineGroup> LinesGroup { get; set; }
    public DbSet<LinePoint> LinePoints { get; set; }

  }
}
