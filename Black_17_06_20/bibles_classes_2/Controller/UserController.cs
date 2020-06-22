using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace bibles_classes_2.Controller {
  /// <summary>
  /// Контроллер пользователя.
  /// </summary>
  public class UserController {
    /// <summary>
    /// Пользователь приложения.
    /// </summary>
    public User User { get; }
    /// <summary>
    /// Создание нового контроллера.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    public UserController( string userName, string genderName, DateTime birthDate, double weight, double height ) {

      var gender = new Gender( genderName );
      User = new User( userName, gender, birthDate, weight, height );

    }


    /// <summary>
    /// Получить данные пользователя
    /// </summary>
    /// <returns>Пользователь приложения.</returns>
    public UserController() {
      var formatter = new BinaryFormatter();

      using ( var fs = new FileStream( "user.dat", FileMode.OpenOrCreate ) )
      {
        if ( formatter.Deserialize( fs ) is User user )
        {
          User = user;
        }


        // TODO: Что делать, усли пользователя не прочитали
      }
    }

    /// <summary>
    /// Сохранить данные пользователя
    /// </summary>
    public void Save() {

      var formatter = new BinaryFormatter();

      using ( var fs = new FileStream( "user.dat", FileMode.OpenOrCreate ) )
      {
        formatter.Serialize( fs, User );
      }


    }
  }
}
