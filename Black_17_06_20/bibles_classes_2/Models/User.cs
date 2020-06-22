using System;
namespace bibles_classes_2 {
  /// <summary>
  /// Пользователь
  /// </summary>
  [Serializable]
  public class User {


    public string Name { get; }

    public Gender Gender { get; }

    public DateTime BirthDate { get; }

    public double Weight { get; set; }

    public double Height { get; set; }

    /// <summary>
    /// Создание Нового Пользователя
    /// </summary>
    /// <param name="name">Имя</param>
    /// <param name="gender">Пол</param>
    /// <param name="birthDate">Дата Рождения</param>
    /// <param name="weight">Вес</param>
    /// <param name="height">Рост</param>
    public User( string name,
                Gender gender,
                DateTime birthDate,
                double weight,
                double height ) {

      #region <Прверка полей>

      if ( string.IsNullOrWhiteSpace( name ) )
      {
        throw new ArgumentNullException( "Имя поля не может быть пустым или null", nameof( name ) );
      }
      if ( gender == null )
      {
        throw new ArgumentNullException( "Пол не может быть null", nameof( gender ) );
      }
      if ( birthDate < DateTime.Parse( "01.01.1900" ) || birthDate >= DateTime.Now )
      {
        throw new ArgumentException( "Невозможная дата рождения", nameof( birthDate ) );
      }

      if ( weight <= 0 )
      {
        throw new ArgumentException( "Вес не может быть меньше или равен нулю", nameof( weight ) );
      }
      if ( height <= 0 )
      {
        throw new ArgumentException( "Ротс не может быть меньше или равен нулю", nameof( height ) );
      }

      Name = name;
      Gender = gender;
      BirthDate = birthDate;
      Weight = weight;
      Height = height;

      #endregion
    }
    public override string ToString() {
      return base.ToString();
    }
  }
}
