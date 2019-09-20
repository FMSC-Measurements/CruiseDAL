using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("CuttingUnitStratum")]
 public partial class CuttingUnitStratum
 {
  [Field("CuttingUnit_CN")]
  public Int32 CuttingUnit_CN { get; set; }

  [Field("Stratum_CN")]
  public Int32 Stratum_CN { get; set; }

  [Field("StratumArea")]
  public Double? StratumArea { get; set; }

 }

}
