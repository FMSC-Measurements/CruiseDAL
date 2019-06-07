using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("CuttingUnit")]
 public partial class CuttingUnit
 {
  [Field("CuttingUnit_CN")]
  public Int64 CuttingUnit_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("Area")]
  public Double Area { get; set; }

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

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Int64 RowVersion { get; set; }

 }

}
