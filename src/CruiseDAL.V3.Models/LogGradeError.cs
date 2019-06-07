using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("LogGradeError")]
 public partial class LogGradeError
 {
  [Field("Log_CN")]
  public Int64 Log_CN { get; set; }

  [Field("LogID")]
  public String LogID { get; set; }

  [Field("LogGradeAuditRule_CN")]
  public Int64 LogGradeAuditRule_CN { get; set; }

  [Field("Message")]
  public Object Message { get; set; }

  [Field("IsResolved")]
  public Object IsResolved { get; set; }

  [Field("Resolution")]
  public Object Resolution { get; set; }

  [Field("ResolutionInitials")]
  public Object ResolutionInitials { get; set; }

 }

}
