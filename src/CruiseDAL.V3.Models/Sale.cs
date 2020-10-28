using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Sale")]
 public partial class Sale
 {
  [PrimaryKeyField("Sale_CN")]
  public Int32? Sale_CN { get; set; }

  [Field("SaleID")]
  public String SaleID { get; set; }

  [Field("SaleNumber")]
  public String SaleNumber { get; set; }

  [Field("Name")]
  public String Name { get; set; }

  [Field("Purpose")]
  public String Purpose { get; set; }

  [Field("Region")]
  public String Region { get; set; }

  [Field("Forest")]
  public String Forest { get; set; }

  [Field("District")]
  public String District { get; set; }

  [Field("MeasurementYear")]
  public String MeasurementYear { get; set; }

  [Field("CalendarYear")]
  public Int32? CalendarYear { get; set; }

  [Field("LogGradingEnabled")]
  public Boolean? LogGradingEnabled { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

  [Field("DefaultUOM")]
  public String DefaultUOM { get; set; }

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
