using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("PRO")]
 public partial class PRO
 {
  [PrimaryKeyField("PRO_CN")]
  public Int32? PRO_CN { get; set; }

  [Field("CutLeave")]
  public String CutLeave { get; set; }

  [Field("Stratum")]
  public String Stratum { get; set; }

  [Field("CuttingUnit")]
  public String CuttingUnit { get; set; }

  [Field("SampleGroup")]
  public String SampleGroup { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("SecondaryProduct")]
  public String SecondaryProduct { get; set; }

  [Field("UOM")]
  public String UOM { get; set; }

  [Field("STM")]
  public String STM { get; set; }

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

  [Field("ProrationFactor")]
  public Double? ProrationFactor { get; set; }

  [Field("ProratedEstimatedTrees")]
  public Double? ProratedEstimatedTrees { get; set; }

 }

}
