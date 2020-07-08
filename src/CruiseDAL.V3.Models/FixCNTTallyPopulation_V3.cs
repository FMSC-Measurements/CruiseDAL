using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("FixCNTTallyPopulation_V3")]
 public partial class FixCNTTallyPopulation_V3
 {
  [PrimaryKeyField("FixCNTTallyPopulation_CN")]
  public Int32? FixCNTTallyPopulation_CN { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("IntervalSize")]
  public Int32? IntervalSize { get; set; }

  [Field("Min")]
  public Int32? Min { get; set; }

  [Field("Max")]
  public Int32? Max { get; set; }

 }

}
