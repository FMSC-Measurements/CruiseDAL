using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LogGradeError")]
 public partial class LogGradeError
 {
  [Field("Log_CN")]
  public Int32? Log_CN { get; set; }

  [Field("LogID")]
  public String LogID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("LogGradeAuditRule_CN")]
  public Int32? LogGradeAuditRule_CN { get; set; }

  [Field("Message")]
  public String Message { get; set; }

  [Field("IsResolved")]
  public String IsResolved { get; set; }

  [Field("Resolution")]
  public String Resolution { get; set; }

  [Field("ResolutionInitials")]
  public String ResolutionInitials { get; set; }

 }

}
