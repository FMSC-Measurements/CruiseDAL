using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("UOMCodes")]
 public partial class UOMCodes
 {
  [PrimaryKeyField("UOMCodes_CN")]
  public Int32? UOMCodes_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("FriendlyValue")]
  public String FriendlyValue { get; set; }

 }

}
