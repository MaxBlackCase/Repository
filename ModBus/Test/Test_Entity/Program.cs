using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Test_Entity.Models;

namespace Test_Entity {
  internal class Program {

    static void Main( string[] args ) {

      using( var context = new SampleContext() ) {

        var lpg = new List<LineGroup>() {
        new LineGroup{NameLine = "Volltage"},
        new LineGroup{NameLine = "Current" },
        new LineGroup{NameLine = "Torque" },
        new LineGroup{NameLine = "External" },
        new LineGroup{NameLine = "Motor" }
        };
        

        lpg.ForEach( s => context.LinesGroup.AddOrUpdate( p => p.NameLine, s ) );
        context.SaveChanges();

        var lp = new List<LinePoint>();
        var rand = new Random();

        for( int i = 0; i < rand.Next( 25, 100 ); i++ ) {
          lp.Add( new LinePoint { LineGroupId = rand.Next( 1, 6 ), Time = TimeSpan.FromSeconds( rand.Next( 500, 9000 ) ), Values = rand.Next( 25, 6230 ) } );
        }
        context.LinePoints.AddRange( lp );
        context.SaveChanges();


        var result = context.LinesGroup.GroupJoin(
          context.LinePoints,
          lpgItem => lpgItem.Id,
          lpItem => lpItem.LineGroupId,
          ( group, line ) => new
          {
            LineName = group.NameLine,
            LineData = line.Select( res => new { res.Time, res.Values } )
          } );

        foreach( var grpItem in result ) {
          Console.WriteLine( $"Line Name: {grpItem.LineName}\n" );
          foreach( var lineItem in grpItem.LineData ) {
            Console.WriteLine( $"\tTime: {lineItem.Time} Value: {lineItem.Values}" );
          }
        }
      }

      Console.Read();
    }

  }
}
