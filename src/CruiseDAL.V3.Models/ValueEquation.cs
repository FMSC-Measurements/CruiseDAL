using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("ValueEquation")]
 public partial class ValueEquation
 {
  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("ValueEquationNumber")]
  public String ValueEquationNumber { get; set; }

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

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("Modified_TS")]
  public DateTime? Modified_TS { get; set; }

 }

}
