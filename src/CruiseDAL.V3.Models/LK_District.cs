using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_District")]
 public partial class LK_District
 {
  [PrimaryKeyField("LK_District_CN")]
  public Int32? LK_District_CN { get; set; }

  [Field("Region")]
  public String Region { get; set; }

  [Field("Forest")]
  public String Forest { get; set; }

  [Field("District")]
  public String District { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
