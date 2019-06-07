using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Stratum")]
 public partial class Stratum
 {
  [Field("Stratum_CN")]
  public Int64 Stratum_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("Method")]
  public String Method { get; set; }

  [Field("BasalAreaFactor")]
  public Double BasalAreaFactor { get; set; }

  [Field("FixedPlotSize")]
  public Double FixedPlotSize { get; set; }

  [Field("KZ3PPNT")]
  public Int64 KZ3PPNT { get; set; }

  [Field("SamplingFrequency")]
  public Int64 SamplingFrequency { get; set; }

  [Field("Hotkey")]
  public String Hotkey { get; set; }

  [Field("FBSCode")]
  public String FBSCode { get; set; }

  [Field("YieldComponent")]
  public String YieldComponent { get; set; }

  [Field("VolumeFactor")]
  public Double VolumeFactor { get; set; }

  [Field("Month")]
  public Int64 Month { get; set; }

  [Field("Year")]
  public Int64 Year { get; set; }

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
