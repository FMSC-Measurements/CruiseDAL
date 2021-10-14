using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeMeasurment_DefaultResolved")]
 public partial class TreeMeasurment_DefaultResolved
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("SeenDefectPrimary")]
  public String SeenDefectPrimary { get; set; }

  [Field("SeenDefectSecondary")]
  public String SeenDefectSecondary { get; set; }

  [Field("RecoverablePrimary")]
  public String RecoverablePrimary { get; set; }

  [Field("HiddenPrimary")]
  public String HiddenPrimary { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

  [Field("HeightToFirstLiveLimb")]
  public String HeightToFirstLiveLimb { get; set; }

  [Field("PoleLength")]
  public String PoleLength { get; set; }

  [Field("ClearFace")]
  public String ClearFace { get; set; }

  [Field("CrownRatio")]
  public String CrownRatio { get; set; }

  [Field("DBH")]
  public String DBH { get; set; }

  [Field("DRC")]
  public String DRC { get; set; }

  [Field("TotalHeight")]
  public String TotalHeight { get; set; }

  [Field("MerchHeightPrimary")]
  public String MerchHeightPrimary { get; set; }

  [Field("MerchHeightSecondary")]
  public String MerchHeightSecondary { get; set; }

  [Field("FormClass")]
  public String FormClass { get; set; }

  [Field("UpperStemDiameter")]
  public String UpperStemDiameter { get; set; }

  [Field("UpperStemHeight")]
  public String UpperStemHeight { get; set; }

  [Field("DBHDoubleBarkThickness")]
  public String DBHDoubleBarkThickness { get; set; }

  [Field("TopDIBPrimary")]
  public String TopDIBPrimary { get; set; }

  [Field("TopDIBSecondary")]
  public String TopDIBSecondary { get; set; }

  [Field("DefectCode")]
  public String DefectCode { get; set; }

  [Field("DiameterAtDefect")]
  public String DiameterAtDefect { get; set; }

  [Field("VoidPercent")]
  public String VoidPercent { get; set; }

  [Field("Slope")]
  public String Slope { get; set; }

  [Field("Aspect")]
  public String Aspect { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

  [Field("IsFallBuckScale")]
  public String IsFallBuckScale { get; set; }

  [Field("MetaData")]
  public String MetaData { get; set; }

  [Field("Initials")]
  public String Initials { get; set; }

 }

}
