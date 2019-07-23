using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeDefaultValue_TreeAuditRule")]
 public partial class TreeDefaultValue_TreeAuditRule
 {
  [PrimaryKeyField("TreeDefaultValue_TreeAuditRule_CN")]
  public Int32? TreeDefaultValue_TreeAuditRule_CN { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

 }

}
