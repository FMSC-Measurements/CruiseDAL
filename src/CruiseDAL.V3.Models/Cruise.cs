using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Cruise")]
 public partial class Cruise
 {
  [PrimaryKeyField("Cruise_CN")]
  public Int32? Cruise_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("SaleID")]
  public String SaleID { get; set; }

  [Field("SaleNumber")]
  public String SaleNumber { get; set; }

  [Field("CruiseNumber")]
  public String CruiseNumber { get; set; }

  [Field("Purpose")]
  public String Purpose { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

  [Field("DefaultUOM")]
  public String DefaultUOM { get; set; }

  [Field("MeasurementYear")]
  public String MeasurementYear { get; set; }

  [Field("LogGradingEnabled")]
  public Boolean? LogGradingEnabled { get; set; }

  [Field("UseCrossStrataPlotTreeNumbering")]
  public Boolean? UseCrossStrataPlotTreeNumbering { get; set; }

  [Field("TemplateFile")]
  public String TemplateFile { get; set; }

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
