using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("CuttingUnit")]
 public partial class CuttingUnit
 {
  [PrimaryKeyField("CuttingUnit_CN")]
  public Int32? CuttingUnit_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("Area")]
  public Double? Area { get; set; }

  [Field("TallyHistory")]
  public String TallyHistory { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("LoggingMethod")]
  public String LoggingMethod { get; set; }

  [Field("PaymentUnit")]
  public String PaymentUnit { get; set; }

  [Field("Rx")]
  public String Rx { get; set; }

 }

}
