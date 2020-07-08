using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("SampleGroupStatsTreeDefaultValue")]
 public partial class SampleGroupStatsTreeDefaultValue
 {
  [Field("TreeDefaultValue_CN")]
  public Int32? TreeDefaultValue_CN { get; set; }

  [Field("SampleGroupStats_CN")]
  public Int32? SampleGroupStats_CN { get; set; }

 }

}
