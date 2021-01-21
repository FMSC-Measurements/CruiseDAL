using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_UOM")]
 public partial class LK_UOM
 {
  [PrimaryKeyField("LK_UOM_CN")]
  public Int32? LK_UOM_CN { get; set; }

  [Field("UOM")]
  public String UOM { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
