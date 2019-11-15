using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Tree")]
 public partial class Tree
 {
  [PrimaryKeyField("Tree_CN")]
  public Int32? Tree_CN { get; set; }

  [Field("Tree_GUID")]
  public String Tree_GUID { get; set; }

  [Field("TreeDefaultValue_CN")]
  public Int32? TreeDefaultValue_CN { get; set; }

  [Field("Stratum_CN")]
  public Int32 Stratum_CN { get; set; }

  [Field("SampleGroup_CN")]
  public Int32? SampleGroup_CN { get; set; }

  [Field("CuttingUnit_CN")]
  public Int32 CuttingUnit_CN { get; set; }

  [Field("Plot_CN")]
  public Int32? Plot_CN { get; set; }

  [Field("TreeNumber")]
  public Int32 TreeNumber { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("CountOrMeasure")]
  public String CountOrMeasure { get; set; }

  [Field("TreeCount")]
  public Double? TreeCount { get; set; }

  [Field("KPI")]
  public Double? KPI { get; set; }

  [Field("STM")]
  public String STM { get; set; }

  [Field("SeenDefectPrimary")]
  public Double? SeenDefectPrimary { get; set; }

  [Field("SeenDefectSecondary")]
  public Double? SeenDefectSecondary { get; set; }

  [Field("RecoverablePrimary")]
  public Double? RecoverablePrimary { get; set; }

  [Field("HiddenPrimary")]
  public Double? HiddenPrimary { get; set; }

  [Field("Initials")]
  public String Initials { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

  [Field("HeightToFirstLiveLimb")]
  public Double? HeightToFirstLiveLimb { get; set; }

  [Field("PoleLength")]
  public Double? PoleLength { get; set; }

  [Field("ClearFace")]
  public String ClearFace { get; set; }

  [Field("CrownRatio")]
  public Double? CrownRatio { get; set; }

  [Field("DBH")]
  public Double? DBH { get; set; }

  [Field("DRC")]
  public Double? DRC { get; set; }

  [Field("TotalHeight")]
  public Double? TotalHeight { get; set; }

  [Field("MerchHeightPrimary")]
  public Double? MerchHeightPrimary { get; set; }

  [Field("MerchHeightSecondary")]
  public Double? MerchHeightSecondary { get; set; }

  [Field("FormClass")]
  public Double? FormClass { get; set; }

  [Field("UpperStemDiameter")]
  public Double? UpperStemDiameter { get; set; }

  [Field("UpperStemHeight")]
  public Double? UpperStemHeight { get; set; }

  [Field("DBHDoubleBarkThickness")]
  public Double? DBHDoubleBarkThickness { get; set; }

  [Field("TopDIBPrimary")]
  public Double? TopDIBPrimary { get; set; }

  [Field("TopDIBSecondary")]
  public Double? TopDIBSecondary { get; set; }

  [Field("DefectCode")]
  public String DefectCode { get; set; }

  [Field("DiameterAtDefect")]
  public Double? DiameterAtDefect { get; set; }

  [Field("VoidPercent")]
  public Double? VoidPercent { get; set; }

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

  [Field("MetaData")]
  public String MetaData { get; set; }

  [Field("IsFallBuckScale")]
  public Int32? IsFallBuckScale { get; set; }

  [Field("ExpansionFactor")]
  public Double? ExpansionFactor { get; set; }

  [Field("TreeFactor")]
  public Double? TreeFactor { get; set; }

  [Field("PointFactor")]
  public Double? PointFactor { get; set; }

 }

}
