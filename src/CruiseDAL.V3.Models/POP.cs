using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("POP")]
 public partial class POP
 {
  [PrimaryKeyField("POP_CN")]
  public Int32? POP_CN { get; set; }

  [Field("CutLeave")]
  public String CutLeave { get; set; }

  [Field("Stratum")]
  public String Stratum { get; set; }

  [Field("SampleGroup")]
  public String SampleGroup { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("SecondaryProduct")]
  public String SecondaryProduct { get; set; }

  [Field("STM")]
  public String STM { get; set; }

  [Field("UOM")]
  public String UOM { get; set; }

  [Field("FirstStageTrees")]
  public Double? FirstStageTrees { get; set; }

  [Field("MeasuredTrees")]
  public Double? MeasuredTrees { get; set; }

  [Field("TalliedTrees")]
  public Double? TalliedTrees { get; set; }

  [Field("SumKPI")]
  public Double? SumKPI { get; set; }

  [Field("SumMeasuredKPI")]
  public Double? SumMeasuredKPI { get; set; }

  [Field("StageOneSamples")]
  public Double? StageOneSamples { get; set; }

  [Field("StageTwoSamples")]
  public Double? StageTwoSamples { get; set; }

  [Field("Stg1GrossXPP")]
  public Double? Stg1GrossXPP { get; set; }

  [Field("Stg1GrossXsqrdPP")]
  public Double? Stg1GrossXsqrdPP { get; set; }

  [Field("Stg1NetXPP")]
  public Double? Stg1NetXPP { get; set; }

  [Field("Stg1NetXsqrdPP")]
  public Double? Stg1NetXsqrdPP { get; set; }

  [Field("Stg1ValueXPP")]
  public Double? Stg1ValueXPP { get; set; }

  [Field("Stg1ValueXsqrdPP")]
  public Double? Stg1ValueXsqrdPP { get; set; }

  [Field("Stg2GrossXPP")]
  public Double? Stg2GrossXPP { get; set; }

  [Field("Stg2GrossXsqrdPP")]
  public Double? Stg2GrossXsqrdPP { get; set; }

  [Field("Stg2NetXPP")]
  public Double? Stg2NetXPP { get; set; }

  [Field("Stg2NetXsqrdPP")]
  public Double? Stg2NetXsqrdPP { get; set; }

  [Field("Stg2ValueXPP")]
  public Double? Stg2ValueXPP { get; set; }

  [Field("Stg2ValueXsqrdPP")]
  public Double? Stg2ValueXsqrdPP { get; set; }

  [Field("Stg1GrossXSP")]
  public Double? Stg1GrossXSP { get; set; }

  [Field("Stg1GrossXsqrdSP")]
  public Double? Stg1GrossXsqrdSP { get; set; }

  [Field("Stg1NetXSP")]
  public Double? Stg1NetXSP { get; set; }

  [Field("Stg1NetXsqrdSP")]
  public Double? Stg1NetXsqrdSP { get; set; }

  [Field("Stg1ValueXSP")]
  public Double? Stg1ValueXSP { get; set; }

  [Field("Stg1ValueXsqrdSP")]
  public Double? Stg1ValueXsqrdSP { get; set; }

  [Field("Stg2GrossXSP")]
  public Double? Stg2GrossXSP { get; set; }

  [Field("Stg2GrossXsqrdSP")]
  public Double? Stg2GrossXsqrdSP { get; set; }

  [Field("Stg2NetXSP")]
  public Double? Stg2NetXSP { get; set; }

  [Field("Stg2NetXsqrdSP")]
  public Double? Stg2NetXsqrdSP { get; set; }

  [Field("Stg2ValueXSP")]
  public Double? Stg2ValueXSP { get; set; }

  [Field("Stg2ValueXsqrdSP")]
  public Double? Stg2ValueXsqrdSP { get; set; }

  [Field("Stg1GrossXRP")]
  public Double? Stg1GrossXRP { get; set; }

  [Field("Stg1GrossXsqrdRP")]
  public Double? Stg1GrossXsqrdRP { get; set; }

  [Field("Stg1NetXRP")]
  public Double? Stg1NetXRP { get; set; }

  [Field("Stg1NetXRsqrdRP")]
  public Double? Stg1NetXRsqrdRP { get; set; }

  [Field("Stg1ValueXRP")]
  public Double? Stg1ValueXRP { get; set; }

  [Field("Stg1ValueXsqrdRP")]
  public Double? Stg1ValueXsqrdRP { get; set; }

  [Field("Stg2GrossXRP")]
  public Double? Stg2GrossXRP { get; set; }

  [Field("Stg2GrossXsqrdRP")]
  public Double? Stg2GrossXsqrdRP { get; set; }

  [Field("Stg2NetXRP")]
  public Double? Stg2NetXRP { get; set; }

  [Field("Stg2NetXsqrdRP")]
  public Double? Stg2NetXsqrdRP { get; set; }

  [Field("Stg2ValueXRP")]
  public Double? Stg2ValueXRP { get; set; }

  [Field("Stg2ValueXsqrdRP")]
  public Double? Stg2ValueXsqrdRP { get; set; }

 }

}
