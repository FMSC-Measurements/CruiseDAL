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

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("IsEmpty")]
  public Boolean? IsEmpty { get; set; }

  [Field("KPI")]
  public Double? KPI { get; set; }

  [Field("ThreePRandomValue")]
  public Int32? ThreePRandomValue { get; set; }

 }

}
