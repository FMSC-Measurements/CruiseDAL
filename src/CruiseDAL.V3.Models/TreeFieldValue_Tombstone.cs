using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeFieldValue_Tombstone")]
 public partial class TreeFieldValue_Tombstone
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("ValueInt")]
  public Int32? ValueInt { get; set; }

  [Field("ValueReal")]
  public Double? ValueReal { get; set; }

  [Field("ValueBool")]
  public Boolean? ValueBool { get; set; }

  [Field("ValueText")]
  public String ValueText { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("Modified_TS")]
  public DateTime? Modified_TS { get; set; }

  [Field("Deleted_TS")]
  public DateTime? Deleted_TS { get; set; }

 }

}
