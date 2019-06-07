using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Plot")]
 public partial class Plot
 {
  [Field("Plot_CN")]
  public Int64 Plot_CN { get; set; }

  [Field("Stratum_CN")]
  public Int64 Stratum_CN { get; set; }

  [Field("CuttingUnit_CN")]
  public Int64 CuttingUnit_CN { get; set; }

  [Field("PlotNumber")]
  public Int64 PlotNumber { get; set; }

  [Field("IsEmpty")]
  public Object IsEmpty { get; set; }

  [Field("Slope")]
  public Double Slope { get; set; }

  [Field("KPI")]
  public Double KPI { get; set; }

  [Field("ThreePRandomValue")]
  public Int64 ThreePRandomValue { get; set; }

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

  [Field("MetaData")]
  public Object MetaData { get; set; }

  [Field("Blob")]
  public Object Blob { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("RowVersion")]
  public Int64 RowVersion { get; set; }

 }

}
