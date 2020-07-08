using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("QualityAdjEquation")]
 public partial class QualityAdjEquation
 {
  [Field("Species")]
  public String Species { get; set; }

  [Field("QualityAdjEq")]
  public String QualityAdjEq { get; set; }

  [Field("Year")]
  public Int32? Year { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

  [Field("Coefficient1")]
  public Double? Coefficient1 { get; set; }

  [Field("Coefficient2")]
  public Double? Coefficient2 { get; set; }

  [Field("Coefficient3")]
  public Double? Coefficient3 { get; set; }

  [Field("Coefficient4")]
  public Double? Coefficient4 { get; set; }

  [Field("Coefficient5")]
  public Double? Coefficient5 { get; set; }

  [Field("Coefficient6")]
  public Double? Coefficient6 { get; set; }

 }

}
