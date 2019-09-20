using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Plot")]
 public partial class Plot
 {
  [PrimaryKeyField("Plot_CN")]
  public Int32? Plot_CN { get; set; }

  [Field("Plot_GUID")]
  public String Plot_GUID { get; set; }

  [Field("Stratum_CN")]
  public Int32 Stratum_CN { get; set; }

  [Field("CuttingUnit_CN")]
  public Int32 CuttingUnit_CN { get; set; }

  [Field("PlotNumber")]
  public Int32 PlotNumber { get; set; }

  [Field("IsEmpty")]
  public String IsEmpty { get; set; }

  [Field("Slope")]
  public Double? Slope { get; set; }

  [Field("KPI")]
  public Double? KPI { get; set; }

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

  [Field("MetaData")]
  public String MetaData { get; set; }

  [Field("Blob")]
  public Byte[] Blob { get; set; }

  [Field("ThreePRandomValue")]
  public Int32? ThreePRandomValue { get; set; }

 }

}
