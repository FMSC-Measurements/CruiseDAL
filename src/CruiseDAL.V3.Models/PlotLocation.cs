using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("PlotLocation")]
 public partial class PlotLocation
 {
  [PrimaryKeyField("PlotLocation_CN")]
  public Int32? PlotLocation_CN { get; set; }

  [Field("PlotID")]
  public String PlotID { get; set; }

  [Field("Latitude")]
  public Double Latitude { get; set; }

  [Field("Longitude")]
  public Double Longitude { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("Modified_TS")]
  public DateTime? Modified_TS { get; set; }

 }

}
