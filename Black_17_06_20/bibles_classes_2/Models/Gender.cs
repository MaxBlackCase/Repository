using System;

namespace bibles_classes_2 {
  /// <summary>
  /// Пол.
  /// </summary>
  [Serializable]
  public class Gender {

    public string Name { get; }

    /// <summary>
    /// Создать Новый Пол.
    /// </summary>
    /// <param name="name">Имя пола.</param>
    public Gender( string name ) {

      if ( string.IsNullOrWhiteSpace( name ) )
      {
        throw new ArgumentNullException( "Имя пола не может быть пустым или null", nameof( name ) );
      }

      Name = name;
    }

    public override string ToString() {
      return base.ToString();
    }
  }
}
