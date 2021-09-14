using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("PlotError")]
 public partial class PlotError
 {
  [Field("PlotID")]
  public String PlotID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("PlotNumber")]
  public Int32? PlotNumber { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("Plot_Stratum_CN")]
  public Int32? Plot_Stratum_CN { get; set; }

  [Field("Message")]
  public String Message { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("Level")]
  public String Level { get; set; }

  [Field("IsResolved")]
  public String IsResolved { get; set; }

  [Field("Resolution")]
  public String Resolution { get; set; }

  [Field("ResolutionInitials")]
  public String ResolutionInitials { get; set; }

 }

}
