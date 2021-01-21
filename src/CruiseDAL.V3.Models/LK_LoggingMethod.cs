using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_LoggingMethod")]
 public partial class LK_LoggingMethod
 {
  [PrimaryKeyField("LK_LoggingMethod_CN")]
  public Int32? LK_LoggingMethod_CN { get; set; }

  [Field("LoggingMethod")]
  public String LoggingMethod { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
