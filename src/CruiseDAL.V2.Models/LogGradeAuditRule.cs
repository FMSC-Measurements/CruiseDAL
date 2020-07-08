using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("LogGradeAuditRule")]
 public partial class LogGradeAuditRule
 {
  [Field("Species")]
  public String Species { get; set; }

  [Field("DefectMax")]
  public Double? DefectMax { get; set; }

  [Field("ValidGrades")]
  public String ValidGrades { get; set; }

 }

}
