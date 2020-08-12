using System;

namespace MyAppModBus.Models.DbModel {
  internal class LinePoint {

    public int Id { get; set; }
    public TimeSpan Time { get; set; }
    public double Values { get; set; }
    public int LineGroupId { get; set; }

  }
}
