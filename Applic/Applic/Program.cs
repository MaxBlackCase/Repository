using System;

namespace Applic
{
  class Person
  {
    public string name;
    public int age = 18;

    public void GetInfo()
    {
      Console.WriteLine($"Имя: {name}\nВозраст: {age}\n\n");

    }
  }
  class Program
  {
    static void Main(string[] args)
    {
      Person tom = new Person();
      tom.GetInfo();

      tom.name = "Tom";
      tom.age = 34;
      tom.GetInfo();
      Console.ReadKey();
    }
  }
}
