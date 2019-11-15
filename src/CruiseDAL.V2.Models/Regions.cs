using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Regions")]
 public partial class Regions
 {
  [PrimaryKeyField("Region_CN")]
  public Int32? Region_CN { get; set; }

  [Field("Number")]
  public Int32? Number { get; set; }

  [Field("Name")]
  public String Name { get; set; }

 }

}
