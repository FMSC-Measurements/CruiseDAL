using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LoggingMethods")]
 public partial class LoggingMethods
 {
  [PrimaryKeyField("LoggingMethods_CN")]
  public Int32? LoggingMethods_CN { get; set; }

  [Field("LoggingMethod")]
  public String LoggingMethod { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
