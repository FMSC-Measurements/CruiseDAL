using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("FixCNTTallyClass")]
 public partial class FixCNTTallyClass
 {
  [PrimaryKeyField("FixCNTTallyClass_CN")]
  public Int32? FixCNTTallyClass_CN { get; set; }

  [Field("Stratum_CN")]
  public Int32 Stratum_CN { get; set; }

  [Field("FieldName")]
  public Int32? FieldName { get; set; }

 }

}
