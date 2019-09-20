using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("LoggingMethods")]
 public partial class LoggingMethods
 {
  [PrimaryKeyField("LoggingMethods_CN")]
  public Int32? LoggingMethods_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("FriendlyValue")]
  public String FriendlyValue { get; set; }

 }

}
