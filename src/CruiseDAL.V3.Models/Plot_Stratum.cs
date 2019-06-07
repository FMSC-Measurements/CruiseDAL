using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Plot_Stratum")]
 public partial class Plot_Stratum
 {
  [Field("Plot_Stratum_CN")]
  public Int64 Plot_Stratum_CN { get; set; }

  [Field("PlotNumber")]
  public Int64 PlotNumber { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("IsEmpty")]
  public Boolean IsEmpty { get; set; }

  [Field("KPI")]
  public Double KPI { get; set; }

  [Field("ThreePRandomValue")]
  public Int64 ThreePRandomValue { get; set; }

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
