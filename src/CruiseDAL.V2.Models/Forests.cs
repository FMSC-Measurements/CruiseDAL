using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Forests")]
 public partial class Forests
 {
  [PrimaryKeyField("Forest_CN")]
  public Int32? Forest_CN { get; set; }

  [Field("Region_CN")]
  public Int32? Region_CN { get; set; }

  [Field("State")]
  public String State { get; set; }

  [Field("Name")]
  public String Name { get; set; }

  [Field("Number")]
  public Int32? Number { get; set; }

 }

}
