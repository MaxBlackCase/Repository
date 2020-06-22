using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bibles_classes_2.Model {
  /// <summary>
  /// Пол
  /// </summary>
  public class Gender {

    public string Name { get; }

    /// <summary>
    /// Создать новый пол.
    /// </summary>
    /// <param name="name">Имя поля.</param>
    public Gender(string name ) {
      if ( string.IsNullOrWhiteSpace(name) )
      {
        throw new ArgumentNullException("Имя поле не может быть пустым или null", nameof(name));
      }


      Name = name;
    }

    public override string ToString() {
      return base.ToString();
    }

  }
}
