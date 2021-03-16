using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Reports_Tombstone")]
 public partial class Reports_Tombstone
 {
  [Field("ReportID")]
  public String ReportID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Selected")]
  public Boolean? Selected { get; set; }

  [Field("Title")]
  public String Title { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("Modified_TS")]
  public DateTime? Modified_TS { get; set; }

  [Field("Deleted_TS")]
  public DateTime? Deleted_TS { get; set; }

 }

}
