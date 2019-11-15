using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("FixCNTTallyPopulation")]
 public partial class FixCNTTallyPopulation
 {
  [PrimaryKeyField("FixCNTTallyPopulation_CN")]
  public Int32? FixCNTTallyPopulation_CN { get; set; }

  [Field("FixCNTTallyClass_CN")]
  public Int32 FixCNTTallyClass_CN { get; set; }

  [Field("SampleGroup_CN")]
  public Int32 SampleGroup_CN { get; set; }

  [Field("TreeDefaultValue_CN")]
  public Int32 TreeDefaultValue_CN { get; set; }

  [Field("IntervalSize")]
  public Int32? IntervalSize { get; set; }

  [Field("Min")]
  public Int32? Min { get; set; }

  [Field("Max")]
  public Int32? Max { get; set; }

 }

}
