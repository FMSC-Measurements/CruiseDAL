using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeAuditRule_Tombstone")]
 public partial class TreeAuditRule_Tombstone
 {
  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("Min")]
  public Double? Min { get; set; }

  [Field("Max")]
  public Double? Max { get; set; }

  [Field("Description")]
  public String Description { get; set; }

 }

}
