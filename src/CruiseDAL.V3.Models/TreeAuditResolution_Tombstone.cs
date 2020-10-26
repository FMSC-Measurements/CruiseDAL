using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeAuditResolution_Tombstone")]
 public partial class TreeAuditResolution_Tombstone
 {
  [Field("CruiseID")]
  public String CruiseID { get; set; }

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
