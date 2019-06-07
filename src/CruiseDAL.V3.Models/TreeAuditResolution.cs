using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeAuditResolution")]
 public partial class TreeAuditResolution
 {
  [Field("TreeAuditResolution_CN")]
  public Int64 TreeAuditResolution_CN { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

  [Field("Resolution")]
  public String Resolution { get; set; }

  [Field("Initials")]
  public String Initials { get; set; }

 }

}
