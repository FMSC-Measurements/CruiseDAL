using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeDefaultValue")]
 public partial class TreeDefaultValue
 {
  [Field("TreeDefaultValue_CN")]
  public Int64 TreeDefaultValue_CN { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("FIAcode")]
  public Int64 FIAcode { get; set; }

  [Field("CullPrimary")]
  public Double CullPrimary { get; set; }

  [Field("HiddenPrimary")]
  public Double HiddenPrimary { get; set; }

  [Field("CullSecondary")]
  public Double CullSecondary { get; set; }

  [Field("HiddenSecondary")]
  public Double HiddenSecondary { get; set; }

  [Field("Recoverable")]
  public Double Recoverable { get; set; }

  [Field("ContractSpecies")]
  public String ContractSpecies { get; set; }

  [Field("TreeGrade")]
  public String TreeGrade { get; set; }

  [Field("MerchHeightLogLength")]
  public Int64 MerchHeightLogLength { get; set; }

  [Field("MerchHeightType")]
  public String MerchHeightType { get; set; }

  [Field("FormClass")]
  public Double FormClass { get; set; }

  [Field("BarkThicknessRatio")]
  public Double BarkThicknessRatio { get; set; }

  [Field("AverageZ")]
  public Double AverageZ { get; set; }

  [Field("ReferenceHeightPercent")]
  public Double ReferenceHeightPercent { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Int64 RowVersion { get; set; }

 }

}
