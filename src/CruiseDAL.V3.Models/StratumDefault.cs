using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("StratumDefault")]
 public partial class StratumDefault
 {
  [PrimaryKeyField("StratumDefault_CN")]
  public Int32? StratumDefault_CN { get; set; }

  [Field("StratumDefaultID")]
  public String StratumDefaultID { get; set; }

  [Field("Region")]
  public String Region { get; set; }

  [Field("Forest")]
  public String Forest { get; set; }

  [Field("District")]
  public String District { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

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

  [Field("FixCNTField")]
  public String FixCNTField { get; set; }

 }

}
