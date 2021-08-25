using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Stem")]
 public partial class Stem
 {
  [PrimaryKeyField("Stem_CN")]
  public Int32? Stem_CN { get; set; }

  [Field("StemID")]
  public String StemID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Diameter")]
  public Double? Diameter { get; set; }

  [Field("DiameterType")]
  public String DiameterType { get; set; }

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
