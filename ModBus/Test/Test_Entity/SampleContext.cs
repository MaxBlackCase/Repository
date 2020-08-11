﻿using System.Data.Entity;
using Test_Entity.Models;

namespace Test_Entity {
  internal class SampleContext : DbContext {

    public SampleContext() :base( "DbConnectionString" ) { }
    public DbSet<LineGroup> LinesGroup { get; set; }
    public DbSet<LinePoint> LinePoints { get; set; }

  }
}
