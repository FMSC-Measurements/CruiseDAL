using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_Region")]
 public partial class LK_Region
 {
  [PrimaryKeyField("LK_Region_CN")]
  public Int32? LK_Region_CN { get; set; }

  [Field("Region")]
  public String Region { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
