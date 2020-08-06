using System;

namespace MyAppModBus.Models.DbModel {
  internal class LineModel {

    public int Id { get; set; }
    public TimeSpan Time { get; set; }
    public double LineValue { get; set; }
  }
}
