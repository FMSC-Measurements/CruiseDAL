using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeFieldValue_All")]
 public partial class TreeFieldValue_All
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("ValueInt")]
  public String ValueInt { get; set; }

  [Field("ValueReal")]
  public String ValueReal { get; set; }

  [Field("ValueBool")]
  public String ValueBool { get; set; }

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

 }

}
