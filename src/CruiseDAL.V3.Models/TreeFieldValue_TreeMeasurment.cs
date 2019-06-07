using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeFieldValue_TreeMeasurment")]
 public partial class TreeFieldValue_TreeMeasurment
 {
  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("DbType")]
  public String DbType { get; set; }

  [Field("ValueReal")]
  public Object ValueReal { get; set; }

  [Field("ValueBool")]
  public Object ValueBool { get; set; }

  [Field("ValueText")]
  public Object ValueText { get; set; }

  [Field("ValueInt")]
  public Object ValueInt { get; set; }

  [Field("CreatedDate")]
  public Object CreatedDate { get; set; }

 }

}
