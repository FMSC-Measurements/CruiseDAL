using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TallyPopulation")]
 public partial class TallyPopulation
 {
  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("Species")]
  public Object Species { get; set; }

  [Field("LiveDead")]
  public Object LiveDead { get; set; }

  [Field("Description")]
  public Object Description { get; set; }

  [Field("HotKey")]
  public Object HotKey { get; set; }

 }

}
