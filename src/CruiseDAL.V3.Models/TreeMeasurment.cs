using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeMeasurment")]
 public partial class TreeMeasurment
 {
  [PrimaryKeyField("TreeMeasurment_CN", PersistanceFlags = PersistanceFlags.OnUpdate)]
  public Int32? TreeMeasurment_CN { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("SeenDefectPrimary")]
  public Double? SeenDefectPrimary { get; set; }

  [Field("SeenDefectSecondary")]
  public Double? SeenDefectSecondary { get; set; }

  [Field("RecoverablePrimary")]
  public Double? RecoverablePrimary { get; set; }

  [Field("HiddenPrimary")]
  public Double? HiddenPrimary { get; set; }

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

  [Field("IsFallBuckScale")]
  public Boolean? IsFallBuckScale { get; set; }

  [Field("MetaData")]
  public String MetaData { get; set; }

  [Field("Initials")]
  public String Initials { get; set; }

 }

}
