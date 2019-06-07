using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("FixCNTTallyPopulation_V3")]
 public partial class FixCNTTallyPopulation_V3
 {
  [Field("FixCNTTallyPopulation_CN")]
  public Int64 FixCNTTallyPopulation_CN { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("IntervalSize")]
  public Int64 IntervalSize { get; set; }

  [Field("Min")]
  public Int64 Min { get; set; }

  [Field("Max")]
  public Int64 Max { get; set; }

 }

}
