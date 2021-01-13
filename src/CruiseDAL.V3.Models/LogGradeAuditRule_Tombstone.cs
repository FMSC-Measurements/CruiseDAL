using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LogGradeAuditRule_Tombstone")]
 public partial class LogGradeAuditRule_Tombstone
 {
  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

  [Field("DefectMax")]
  public Double? DefectMax { get; set; }

  [Field("Deleted_TS")]
  public DateTime? Deleted_TS { get; set; }

 }

}
