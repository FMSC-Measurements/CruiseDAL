using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("TreeAuditValue")]
 public partial class TreeAuditValue
 {
  [PrimaryKeyField("TreeAuditValue_CN")]
  public Int32? TreeAuditValue_CN { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("Min")]
  public Double? Min { get; set; }

  [Field("Max")]
  public Double? Max { get; set; }

  [Field("ValueSet")]
  public String ValueSet { get; set; }

  [Field("Required")]
  public Boolean? Required { get; set; }

 }

}
