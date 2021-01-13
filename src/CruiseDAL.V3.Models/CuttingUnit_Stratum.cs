using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("CuttingUnit_Stratum")]
 public partial class CuttingUnit_Stratum
 {
  [PrimaryKeyField("CuttingUnit_Stratum_CN")]
  public Int32? CuttingUnit_Stratum_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("StratumArea")]
  public Double? StratumArea { get; set; }

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
