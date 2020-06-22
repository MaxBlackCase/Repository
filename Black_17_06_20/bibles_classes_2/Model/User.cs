using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace bibles_classes_2.Model {

  /// <summary>
  /// Класс Пользователь.
  /// </summary>
  public class User {

    /// <summary>
    /// Имя.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Гендер.
    /// </summary>
    public Gender Gender { get; }

    /// <summary>
    /// Дата рождения.
    /// </summary>
    public DateTime BirthDate { get; }

    /// <summary>
    /// Вес.
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// Рост.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Создать нового пользователя.
    /// </summary>
    /// <param name="name">Имя.</param>
    /// <param name="gender">Гендер.</param>
    /// <param name="birthDate">Дата рождения.</param>
    /// <param name="weight">Вес.</param>
    /// <param name="height">Рост.</param>
    public User(string name, 
                Gender gender, 
                DateTime birthDate, 
                double weight, 
                double height ) {

      #region <Проверка условий>
      if ( string.IsNullOrWhiteSpace(name) )
      {
        throw new ArgumentException( "Имя пользователя не может быть пусты или null", nameof(name));
      }
      if ( gender == null )
      {
        throw new ArgumentException( "Пол не может быть null", nameof(gender));

      }

      if ( birthDate < DateTime.Parse("01.01.1900") || birthDate >= DateTime.Now)
      {
        throw new ArgumentException( "Невозможная дата рождения", nameof( birthDate ) );
      }
      if ( weight <= 0 )
      {
        throw new ArgumentException( "Вес не может быть меньше или равен 0", nameof( weight ) );
      }
      if ( height <=0 )
      {
        throw new ArgumentException( "Рост не может быть меньше или равен 0", nameof( height ) );
      }

      #endregion

      Name = name;
      Gender = gender;
      BirthDate = birthDate;
      Weight = weight;
      Height = height;

    }

    public override string ToString() {
      return Name;
    }

  }
}
