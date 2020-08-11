using System;
using System.Collections.Generic;

namespace FirstCode {
  public class LinePoint {

    public int Id { get; set; }
    public TimeSpan Time { get; set; }
    public double Values { get; set; }
    public int? LineGroupId { get; set; }
    public virtual LineGroup LineGroup { get; set; }

  }
  public class LineGroup {

    public int Id { get; set; }
    public string NameLine { get; set; }
    public virtual ICollection<LinePoint> LinePoints { get; set; }

    public LineGroup() {
      LinePoints = new List<LinePoint>();
    }
    public override string ToString() {

      return NameLine;
    }

  }
}
