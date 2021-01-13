using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LogGradeAuditRule")]
 public partial class LogGradeAuditRule
 {
  [PrimaryKeyField("LogGradeAuditRule_CN")]
  public Int32? LogGradeAuditRule_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

  [Field("DefectMax")]
  public Double? DefectMax { get; set; }

 }

}
