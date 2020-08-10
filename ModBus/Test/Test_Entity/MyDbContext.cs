using System.Data.Entity;

namespace Test_Entity {
  internal class MyDbContext : DbContext {

    public MyDbContext() :base( "DbConnectionString" ) {

    }

  }
}
