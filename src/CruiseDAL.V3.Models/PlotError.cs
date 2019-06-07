using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("PlotError")]
 public partial class PlotError
 {
  [Field("PlotID")]
  public String PlotID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("PlotNumber")]
  public Int64 PlotNumber { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("Plot_Stratum_CN")]
  public Int64 Plot_Stratum_CN { get; set; }

  [Field("Message")]
  public Object Message { get; set; }

  [Field("Field")]
  public Object Field { get; set; }

  [Field("Level")]
  public Object Level { get; set; }

  [Field("IsResolved")]
  public Object IsResolved { get; set; }

  [Field("Resolution")]
  public Object Resolution { get; set; }

  [Field("ResolutionInitials")]
  public Object ResolutionInitials { get; set; }

 }

}
