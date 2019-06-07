using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("SampleGroupStatsTreeDefaultValue")]
 public partial class SampleGroupStatsTreeDefaultValue
 {
  [Field("TreeDefaultValue_CN")]
  public Int64 TreeDefaultValue_CN { get; set; }

  [Field("SampleGroupStats_CN")]
  public Int64 SampleGroupStats_CN { get; set; }

 }

}
