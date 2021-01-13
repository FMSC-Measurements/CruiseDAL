using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeDefaultValue")]
 public partial class TreeDefaultValue
 {
  [PrimaryKeyField("TreeDefaultValue_CN")]
  public Int32? TreeDefaultValue_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("CullPrimary")]
  public Double? CullPrimary { get; set; }

  [Field("CullPrimaryDead")]
  public Double? CullPrimaryDead { get; set; }

  [Field("HiddenPrimary")]
  public Double? HiddenPrimary { get; set; }

  [Field("HiddenPrimaryDead")]
  public Double? HiddenPrimaryDead { get; set; }

  [Field("TreeGrade")]
  public String TreeGrade { get; set; }

  [Field("TreeGradeDead")]
  public String TreeGradeDead { get; set; }

  [Field("CullSecondary")]
  public Double? CullSecondary { get; set; }

  [Field("HiddenSecondary")]
  public Double? HiddenSecondary { get; set; }

  [Field("Recoverable")]
  public Double? Recoverable { get; set; }

  [Field("MerchHeightLogLength")]
  public Int32? MerchHeightLogLength { get; set; }

  [Field("MerchHeightType")]
  public String MerchHeightType { get; set; }

  [Field("FormClass")]
  public Double? FormClass { get; set; }

  [Field("BarkThicknessRatio")]
  public Double? BarkThicknessRatio { get; set; }

  [Field("AverageZ")]
  public Double? AverageZ { get; set; }

  [Field("ReferenceHeightPercent")]
  public Double? ReferenceHeightPercent { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("Modified_TS")]
  public DateTime? Modified_TS { get; set; }

 }

}
