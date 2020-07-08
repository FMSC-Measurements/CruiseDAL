using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("CuttingUnit_Stratum")]
 public partial class CuttingUnit_Stratum
 {
  [PrimaryKeyField("CuttingUnit_Stratum_CN")]
  public Int32? CuttingUnit_Stratum_CN { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("StratumArea")]
  public Double? StratumArea { get; set; }

 }

}
