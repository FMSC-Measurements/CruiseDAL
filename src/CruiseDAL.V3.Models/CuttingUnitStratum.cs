using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("CuttingUnitStratum")]
 public partial class CuttingUnitStratum
 {
  [Field("CuttingUnit_CN")]
  public Int64 CuttingUnit_CN { get; set; }

  [Field("Stratum_CN")]
  public Int64 Stratum_CN { get; set; }

  [Field("StratumArea")]
  public Double StratumArea { get; set; }

 }

}
