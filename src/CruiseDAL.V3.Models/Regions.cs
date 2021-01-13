using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Regions")]
 public partial class Regions
 {
  [PrimaryKeyField("Regions_CN")]
  public Int32? Regions_CN { get; set; }

  [Field("Region")]
  public String Region { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
