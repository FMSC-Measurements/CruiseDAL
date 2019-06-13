using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeAuditRule")]
 public partial class TreeAuditRule
 {
  [PrimaryKeyField("TreeAuditRule_CN", PersistanceFlags = PersistanceFlags.OnUpdate)]
  public Int32? TreeAuditRule_CN { get; set; }

  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("Min")]
  public Double? Min { get; set; }

  [Field("Max")]
  public Double? Max { get; set; }

 }

}
