using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("CuttingUnit")]
 public partial class CuttingUnit
 {
  [PrimaryKeyField("CuttingUnit_CN")]
  public Int32? CuttingUnit_CN { get; set; }

  [Field("CuttingUnitID")]
  public String CuttingUnitID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Area")]
  public Double? Area { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("LoggingMethod")]
  public String LoggingMethod { get; set; }

  [Field("PaymentUnit")]
  public String PaymentUnit { get; set; }

  [Field("Rx")]
  public String Rx { get; set; }

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
