using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("StratumStats")]
 public partial class StratumStats
 {
  [PrimaryKeyField("StratumStats_CN")]
  public Int32? StratumStats_CN { get; set; }

  [Field("Stratum_CN")]
  public Int32? Stratum_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("Method")]
  public String Method { get; set; }

  [Field("SgSet")]
  public Int32? SgSet { get; set; }

  [Field("SgSetDescription")]
  public String SgSetDescription { get; set; }

  [Field("BasalAreaFactor")]
  public Double? BasalAreaFactor { get; set; }

  [Field("FixedPlotSize")]
  public Double? FixedPlotSize { get; set; }

  [Field("StrError")]
  public Double? StrError { get; set; }

  [Field("SampleSize1")]
  public Int32? SampleSize1 { get; set; }

  [Field("SampleSize2")]
  public Int32? SampleSize2 { get; set; }

  [Field("WeightedCV1")]
  public Double? WeightedCV1 { get; set; }

  [Field("WeightedCV2")]
  public Double? WeightedCV2 { get; set; }

  [Field("TreesPerAcre")]
  public Double? TreesPerAcre { get; set; }

  [Field("VolumePerAcre")]
  public Double? VolumePerAcre { get; set; }

  [Field("TotalVolume")]
  public Double? TotalVolume { get; set; }

  [Field("TotalAcres")]
  public Double? TotalAcres { get; set; }

  [Field("PlotSpacing")]
  public Int32? PlotSpacing { get; set; }

  [Field("Used")]
  public Int32? Used { get; set; }

 }

}
