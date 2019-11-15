using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("SampleGroupStats")]
 public partial class SampleGroupStats
 {
  [PrimaryKeyField("SampleGroupStats_CN")]
  public Int32? SampleGroupStats_CN { get; set; }

  [Field("StratumStats_CN")]
  public Int32? StratumStats_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("SgSet")]
  public Int32? SgSet { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("CutLeave")]
  public String CutLeave { get; set; }

  [Field("UOM")]
  public String UOM { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("SecondaryProduct")]
  public String SecondaryProduct { get; set; }

  [Field("DefaultLiveDead")]
  public String DefaultLiveDead { get; set; }

  [Field("SgError")]
  public Double? SgError { get; set; }

  [Field("SampleSize1")]
  public Int32? SampleSize1 { get; set; }

  [Field("SampleSize2")]
  public Int32? SampleSize2 { get; set; }

  [Field("CV1")]
  public Double? CV1 { get; set; }

  [Field("CV2")]
  public Double? CV2 { get; set; }

  [Field("TreesPerAcre")]
  public Double? TreesPerAcre { get; set; }

  [Field("VolumePerAcre")]
  public Double? VolumePerAcre { get; set; }

  [Field("TreesPerPlot")]
  public Double? TreesPerPlot { get; set; }

  [Field("AverageHeight")]
  public Double? AverageHeight { get; set; }

  [Field("SamplingFrequency")]
  public Int32? SamplingFrequency { get; set; }

  [Field("InsuranceFrequency")]
  public Int32? InsuranceFrequency { get; set; }

  [Field("KZ")]
  public Int32? KZ { get; set; }

  [Field("BigBAF")]
  public Double? BigBAF { get; set; }

  [Field("BigFIX")]
  public Int32? BigFIX { get; set; }

  [Field("MinDbh")]
  public Double? MinDbh { get; set; }

  [Field("MaxDbh")]
  public Double? MaxDbh { get; set; }

  [Field("CV_Def")]
  public Int32? CV_Def { get; set; }

  [Field("CV2_Def")]
  public Int32? CV2_Def { get; set; }

  [Field("TPA_Def")]
  public Int32? TPA_Def { get; set; }

  [Field("VPA_Def")]
  public Int32? VPA_Def { get; set; }

  [Field("ReconPlots")]
  public Int32? ReconPlots { get; set; }

  [Field("ReconTrees")]
  public Int32? ReconTrees { get; set; }

 }

}
