using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gear_array
{
  class Program
  {
    static void Main(string[] args)
    {
      int[][] myArr = new int[3][];
      myArr[0] = new int[5];
      myArr[1] = new int[2];
      myArr[2] = new int[10];

      Random random = new Random();
      Console.WriteLine("\n");
      for (int i = 0; i < myArr.Length; i++)
      {
        for (int j = 0; j < myArr[i].Length; j++)
        {
          myArr[i][j] = random.Next(100);
        }
      }
      for (int i = 0; i < myArr.Length; i++)
      {
        for (int j = 0; j < myArr[i].Length; j++)
        {
          Console.Write(" " + myArr[i][j]);
        }
        Console.WriteLine("\n");
      }
      Console.ReadKey();
    }
  }
}
