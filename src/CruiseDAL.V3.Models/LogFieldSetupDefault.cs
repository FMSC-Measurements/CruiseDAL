using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LogFieldSetupDefault")]
 public partial class LogFieldSetupDefault
 {
  [Field("LogFieldSetupDefault_CN")]
  public Int32? LogFieldSetupDefault_CN { get; set; }

  [Field("StratumDefaultID")]
  public String StratumDefaultID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("FieldOrder")]
  public Int32? FieldOrder { get; set; }

 }

}
