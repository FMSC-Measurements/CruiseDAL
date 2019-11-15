using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("ProductCodes")]
 public partial class ProductCodes
 {
  [PrimaryKeyField("ProductCodes_CN")]
  public Int32? ProductCodes_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("FriendlyValue")]
  public String FriendlyValue { get; set; }

 }

}
