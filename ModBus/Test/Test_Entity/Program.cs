using System;
using System.Collections.Generic;
using System.Linq;
using CodeFirst;

namespace Test_Entity {
  internal class Program {
    static void Main( string[] args ) {

      using( var context = new SampleContext() ) {

        var lgp = new List<LineGroup>()
        {
          new LineGroup{NameLine = "Volltage" },
          new LineGroup{NameLine = "Current" },
          new LineGroup{NameLine = "Torque" },
          new LineGroup{NameLine = "External" },
          new LineGroup{NameLine = "Motor" }
        };

        context.LinesGroup.AddRange( lgp );
        context.SaveChanges();

        var lp = new List<LinePoint>();
        var rand = new Random();
        foreach( var linePointItem in lgp ) {

          lp.Add( new LinePoint { Time = TimeSpan.FromSeconds(rand.Next(1000, 5000)), Values = rand.Next(21, 512), LineGroup = linePointItem.Id} );

        }

        context.LinePoints.AddRange( lp );
        context.SaveChanges();


        Console.WriteLine("OK");
        Console.Read();
      }

    }
  }
}
