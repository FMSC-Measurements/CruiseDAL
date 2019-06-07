using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Plot_V3")]
 public partial class Plot_V3
 {
  [Field("Plot_CN")]
  public Int64 Plot_CN { get; set; }

  [Field("PlotID")]
  public String PlotID { get; set; }

  [Field("PlotNumber")]
  public Int64 PlotNumber { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("Slope")]
  public Double Slope { get; set; }

  [Field("Aspect")]
  public Double Aspect { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

  [Field("XCoordinate")]
  public Double XCoordinate { get; set; }

  [Field("YCoordinate")]
  public Double YCoordinate { get; set; }

  [Field("ZCoordinate")]
  public Double ZCoordinate { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Object RowVersion { get; set; }

 }

}
