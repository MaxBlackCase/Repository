using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Test_Entity.Models {
  internal class LineGroup {
    public int Id { get; set; }
    public string NameLine { get; set; }
    public virtual ICollection<LinePoint> LinePoints { get; set; }

  }
}
