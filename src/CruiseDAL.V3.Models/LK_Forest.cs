using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_Forest")]
 public partial class LK_Forest
 {
  [PrimaryKeyField("LK_Forest_CN")]
  public Int32? LK_Forest_CN { get; set; }

  [Field("Forest")]
  public String Forest { get; set; }

  [Field("Region")]
  public String Region { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
