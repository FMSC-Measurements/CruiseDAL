using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeAuditError")]
 public partial class TreeAuditError
 {
  [Field("Tree_CN")]
  public Int64 Tree_CN { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("IsResolved")]
  public Object IsResolved { get; set; }

  [Field("Message")]
  public Object Message { get; set; }

  [Field("Resolution")]
  public String Resolution { get; set; }

 }

}
