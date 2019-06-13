using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Plot_V3")]
 public partial class Plot_V3
 {
  [PrimaryKeyField("Plot_CN", PersistanceFlags = PersistanceFlags.OnUpdate)]
  public Int32? Plot_CN { get; set; }

  [Field("PlotID")]
  public String PlotID { get; set; }

  [Field("PlotNumber")]
  public Int32 PlotNumber { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("Slope")]
  public Double? Slope { get; set; }

  [Field("Aspect")]
  public Double? Aspect { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

  [Field("XCoordinate")]
  public Double? XCoordinate { get; set; }

  [Field("YCoordinate")]
  public Double? YCoordinate { get; set; }

  [Field("ZCoordinate")]
  public Double? ZCoordinate { get; set; }

 }

}
