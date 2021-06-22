using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("StratumTemplate")]
 public partial class StratumTemplate
 {
  [PrimaryKeyField("StratumTemplate_CN")]
  public Int32? StratumTemplate_CN { get; set; }

  [Field("StratumTemplateName")]
  public String StratumTemplateName { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

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
