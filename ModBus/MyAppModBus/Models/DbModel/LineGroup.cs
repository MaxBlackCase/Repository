using System.Collections.Generic;

namespace MyAppModBus.Models.DbModel {
  internal class LineGroup {

    public int Id { get; set; }
    public string NameLine { get; set; }
    public virtual ICollection<LinePoint> LinePoints { get; set; }

  }
}
