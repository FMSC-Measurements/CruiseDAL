using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("LogMatrix")]
 public partial class LogMatrix
 {
  [Field("ReportNumber")]
  public String ReportNumber { get; set; }

  [Field("GradeDescription")]
  public String GradeDescription { get; set; }

  [Field("LogSortDescription")]
  public String LogSortDescription { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LogGrade1")]
  public String LogGrade1 { get; set; }

  [Field("LogGrade2")]
  public String LogGrade2 { get; set; }

  [Field("LogGrade3")]
  public String LogGrade3 { get; set; }

  [Field("LogGrade4")]
  public String LogGrade4 { get; set; }

  [Field("LogGrade5")]
  public String LogGrade5 { get; set; }

  [Field("LogGrade6")]
  public String LogGrade6 { get; set; }

  [Field("SEDlimit")]
  public String SEDlimit { get; set; }

  [Field("SEDminimum")]
  public Double? SEDminimum { get; set; }

  [Field("SEDmaximum")]
  public Double? SEDmaximum { get; set; }

 }

}
