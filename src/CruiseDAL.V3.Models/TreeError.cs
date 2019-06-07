using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeError")]
 public partial class TreeError
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("TreeAuditRuleID")]
  public Object TreeAuditRuleID { get; set; }

  [Field("Level")]
  public Object Level { get; set; }

  [Field("Message")]
  public Object Message { get; set; }

  [Field("Field")]
  public Object Field { get; set; }

  [Field("IsResolved")]
  public Object IsResolved { get; set; }

  [Field("Resolution")]
  public Object Resolution { get; set; }

  [Field("ResolutionInitials")]
  public Object ResolutionInitials { get; set; }

 }

}
