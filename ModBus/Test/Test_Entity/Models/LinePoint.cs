using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test_Entity.Models {
  class LinePoint {

    public int Id { get; set; }
    public TimeSpan Time { get; set; }
    public double Values { get; set; }
    public int? LineGroupId { get; set; }

  }
}
