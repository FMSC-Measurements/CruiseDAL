using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("SampleGroupTreeDefaultValue")]
 public partial class SampleGroupTreeDefaultValue
 {
  [Field("TreeDefaultValue_CN")]
  public Int32? TreeDefaultValue_CN { get; set; }

  [Field("SampleGroup_CN")]
  public Int32? SampleGroup_CN { get; set; }

 }

}
