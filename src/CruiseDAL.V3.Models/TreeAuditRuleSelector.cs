using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeAuditRuleSelector")]
 public partial class TreeAuditRuleSelector
 {
  [PrimaryKeyField("TreeAuditRuleSelector_CN")]
  public Int32? TreeAuditRuleSelector_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("TreeAuditRuleID")]
  public String TreeAuditRuleID { get; set; }

 }

}
