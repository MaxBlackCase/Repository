using System;
using System.Collections.Generic;

namespace CodeFirst {

  public class LineGroup {
    public int Id { get; set; }
    public string NameLine { get; set; }
    public ICollection<LinePoint> LinePoints { get; set; }

  }
  public class LinePoint {
    public int Id { get; set; }
    public TimeSpan Time { get; set; }
    public double Values { get; set; }
    public int LineGroup { get; set; }

  }
}
