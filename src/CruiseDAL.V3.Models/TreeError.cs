using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeError")]
 public partial class TreeError
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

  [Field("Level")]
  public String Level { get; set; }

  [Field("Message")]
  public String Message { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("IsResolved")]
  public String IsResolved { get; set; }

  [Field("Resolution")]
  public String Resolution { get; set; }

  [Field("ResolutionInitials")]
  public String ResolutionInitials { get; set; }

 }

}
