using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Sale")]
 public partial class Sale
 {
  [Field("Sale_CN")]
  public Int64 Sale_CN { get; set; }

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
  public Int64 CalendarYear { get; set; }

  [Field("LogGradingEnabled")]
  public Boolean LogGradingEnabled { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

  [Field("DefaultUOM")]
  public String DefaultUOM { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Int64 RowVersion { get; set; }

 }

}
