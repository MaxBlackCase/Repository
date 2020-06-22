using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace three_dimensional_array
{
  class Program
  {
    static void Main(string[] args)
    {
      int[,,] myArr = new int[4, 3, 5];

      Random random = new Random();

      for (int i = 0; i < myArr.GetLength(0); i++)
      {

        for (int j = 0; j < myArr.GetLength(1); j++)
        {
          for (int k = 0; k < myArr.GetLength(2); k++)
          {
            myArr[i, j, k] = random.Next(100);
          }
        }
      }
      for (int i = 0; i < myArr.GetLength(0); i++)
      {
        Console.Write( " Array lvl " + (i + 1) + "\n\n" );
        for (int j = 0; j < myArr.GetLength(1); j++)
        {
          Console.Write((j + 1) + " | ");
          for (int k = 0; k < myArr.GetLength(2); k++)
          {
            Console.Write(myArr[i, j, k] + " ");
          }
          Console.WriteLine("\n");
        }
        Console.WriteLine();
      }

      Console.ReadKey();
    }
  }
}
