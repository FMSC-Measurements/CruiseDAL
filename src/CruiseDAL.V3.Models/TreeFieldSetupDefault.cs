using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeFieldSetupDefault")]
 public partial class TreeFieldSetupDefault
 {
  [Field("TreeFieldSetupDefault_CN")]
  public String TreeFieldSetupDefault_CN { get; set; }

  [Field("StratumDefaultID")]
  public String StratumDefaultID { get; set; }

  [Field("SampleGroupDefaultID")]
  public String SampleGroupDefaultID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int32? FieldOrder { get; set; }

  [Field("IsHidden")]
  public Boolean? IsHidden { get; set; }

  [Field("IsLocked")]
  public Boolean? IsLocked { get; set; }

  [Field("DefaultValueInt")]
  public Int32? DefaultValueInt { get; set; }

  [Field("DefaultValueReal")]
  public Double? DefaultValueReal { get; set; }

  [Field("DefaultValueBool")]
  public Boolean? DefaultValueBool { get; set; }

  [Field("DefaultValueText")]
  public String DefaultValueText { get; set; }

 }

}
