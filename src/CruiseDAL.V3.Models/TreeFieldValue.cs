using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeFieldValue")]
 public partial class TreeFieldValue
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("ValueInt")]
  public Int64 ValueInt { get; set; }

  [Field("ValueReal")]
  public Double ValueReal { get; set; }

  [Field("ValueBool")]
  public Boolean ValueBool { get; set; }

  [Field("ValueText")]
  public String ValueText { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

 }

}
