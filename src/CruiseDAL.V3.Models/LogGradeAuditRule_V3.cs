using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LogGradeAuditRule_V3")]
 public partial class LogGradeAuditRule_V3
 {
  [PrimaryKeyField("LogGradeAuditRule_CN")]
  public Int32? LogGradeAuditRule_CN { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("DefectMax")]
  public Double? DefectMax { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

 }

}
