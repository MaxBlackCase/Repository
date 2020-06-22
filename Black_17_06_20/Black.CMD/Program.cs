using bibles_classes_2.Controller;
using System;


namespace Black.CMD {
  class Program {
    static void Main( string[] args ) {

      Console.WriteLine( "Вас приветствует приложение Black" );

      Console.WriteLine( "Введите имя пользователя" );
      var name = Console.ReadLine();

      Console.WriteLine( "Введите пол пользователя" );
      var gender = Console.ReadLine();

      Console.WriteLine( "Введите дату рождения" );
      var birthDate = DateTime.Parse( Console.ReadLine() );

      Console.WriteLine( "Введите вес" );
      var weight = Double.Parse( Console.ReadLine() );

      Console.WriteLine( "Введите рост" );
      var height = Double.Parse( Console.ReadLine() );

      var userController = new UserController(name, gender, birthDate, weight, height);
      userController.Save();




      Console.ReadLine();
    }

    
  }

}
