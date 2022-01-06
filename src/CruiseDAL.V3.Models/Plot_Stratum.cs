using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Plot_Stratum")]
 public partial class Plot_Stratum
 {
  [PrimaryKeyField("Plot_Stratum_CN")]
  public Int32? Plot_Stratum_CN { get; set; }

  [Field("PlotNumber")]
  public Int32 PlotNumber { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("IsEmpty")]
  public Boolean? IsEmpty { get; set; }

  [Field("CountOrMeasure")]
  public String CountOrMeasure { get; set; }

  [Field("TreeCount")]
  public Int32? TreeCount { get; set; }

  [Field("AverageHeight")]
  public Double? AverageHeight { get; set; }

  [Field("KPI")]
  public Double? KPI { get; set; }

  [Field("ThreePRandomValue")]
  public Int32? ThreePRandomValue { get; set; }

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
