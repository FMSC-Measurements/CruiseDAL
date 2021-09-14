using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TallyLedger_Tree_Totals")]
 public partial class TallyLedger_Tree_Totals
 {
  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("STM")]
  public String STM { get; set; }

  [Field("TreeCount")]
  public String TreeCount { get; set; }

  [Field("KPI")]
  public String KPI { get; set; }

 }

}
