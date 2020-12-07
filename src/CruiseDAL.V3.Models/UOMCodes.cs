using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("UOMCodes")]
 public partial class UOMCodes
 {
  [PrimaryKeyField("UOMCodes_CN")]
  public Int32? UOMCodes_CN { get; set; }

  [Field("UOM")]
  public String UOM { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
