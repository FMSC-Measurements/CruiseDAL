using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("CruiseMethods")]
 public partial class CruiseMethods
 {
  [PrimaryKeyField("CruiseMethods_CN")]
  public Int32? CruiseMethods_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("FriendlyValue")]
  public String FriendlyValue { get; set; }

 }

}
