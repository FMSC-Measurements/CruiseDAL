using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("LogGradeAuditRule_V3")]
 public partial class LogGradeAuditRule_V3
 {
  [Field("LogGradeAuditRule_CN")]
  public Int64 LogGradeAuditRule_CN { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("DefectMax")]
  public Double DefectMax { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

 }

}
