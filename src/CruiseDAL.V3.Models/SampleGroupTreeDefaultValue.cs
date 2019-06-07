using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("SampleGroupTreeDefaultValue")]
 public partial class SampleGroupTreeDefaultValue
 {
  [Field("SampleGroup_CN")]
  public Int64 SampleGroup_CN { get; set; }

  [Field("TreeDefaultValue_CN")]
  public Int64 TreeDefaultValue_CN { get; set; }

 }

}
