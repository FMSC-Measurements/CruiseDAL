using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Stratum")]
 public partial class Stratum
 {
  [PrimaryKeyField("Stratum_CN")]
  public Int32? Stratum_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("Method")]
  public String Method { get; set; }

  [Field("BasalAreaFactor")]
  public Double? BasalAreaFactor { get; set; }

  [Field("FixedPlotSize")]
  public Double? FixedPlotSize { get; set; }

  [Field("KZ3PPNT")]
  public Int32? KZ3PPNT { get; set; }

  [Field("SamplingFrequency")]
  public Int32? SamplingFrequency { get; set; }

  [Field("Hotkey")]
  public String Hotkey { get; set; }

  [Field("FBSCode")]
  public String FBSCode { get; set; }

  [Field("YieldComponent")]
  public String YieldComponent { get; set; }

  [Field("VolumeFactor")]
  public Double? VolumeFactor { get; set; }

  [Field("Month")]
  public Int32? Month { get; set; }

  [Field("Year")]
  public Int32? Year { get; set; }

 }

}
