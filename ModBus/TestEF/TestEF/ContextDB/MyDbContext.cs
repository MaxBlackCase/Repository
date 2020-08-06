using System.Data.Entity;
using TestEF.Models;

namespace TestEF.ContextDB {
  internal class MyDbContext : DbContext {
    public MyDbContext() : base( "DbConnectionString" ) { }
    public DbSet<PlayerModel> Players { get; set; }

  }
}
