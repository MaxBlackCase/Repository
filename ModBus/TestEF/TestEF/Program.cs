using System;
using TestEF.Models;

namespace TestEF {
  internal class Program {
    static void Main( string[] args ) {

      using( var context = new MyDbContex() ) {

        var groupNameArr = new string[] { "Rammstein", "Metallica", "Limbizkit", "Dio", "Iron Maiden" };

        for( int i = 0; i < groupNameArr.Length; i++ ) {
          context.Groups.Add( new GroupModel { NameGroup = groupNameArr[ i ], Year = i } );
        }
        context.SaveChanges();

        foreach( var group in context.Groups ) {
          Console.WriteLine( $"id: {group.Id}\nName: {group.NameGroup}\nYear: {group.Year}" );
        }

        Console.Read();
      }

    }

  }
}
