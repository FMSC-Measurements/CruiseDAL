using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeLocation_Tombstone")]
 public partial class TreeLocation_Tombstone
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Latitude")]
  public Double Latitude { get; set; }

  [Field("Longitude")]
  public Double Longitude { get; set; }

  [Field("SS_Latatude")]
  public Double? SS_Latatude { get; set; }

  [Field("SS_Longitude")]
  public Double? SS_Longitude { get; set; }

  [Field("Azimuth")]
  public Double? Azimuth { get; set; }

  [Field("Distance")]
  public Double? Distance { get; set; }

  [Field("IsEstimate")]
  public Boolean? IsEstimate { get; set; }

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
