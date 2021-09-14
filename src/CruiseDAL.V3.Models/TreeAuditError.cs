using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeAuditError")]
 public partial class TreeAuditError
 {
  [Field("Tree_CN")]
  public Int32? Tree_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("IsResolved")]
  public String IsResolved { get; set; }

  [Field("Message")]
  public String Message { get; set; }

  [Field("Resolution")]
  public String Resolution { get; set; }

 }

}
