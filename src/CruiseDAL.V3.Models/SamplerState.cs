using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("SamplerState")]
 public partial class SamplerState
 {
  [PrimaryKeyField("SamplerState_CN")]
  public Int32? SamplerState_CN { get; set; }

  [Field("DeviceID")]
  public String DeviceID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("SampleSelectorType")]
  public String SampleSelectorType { get; set; }

  [Field("BlockState")]
  public String BlockState { get; set; }

  [Field("SystematicIndex")]
  public Int32? SystematicIndex { get; set; }

  [Field("Counter")]
  public Int32? Counter { get; set; }

  [Field("InsuranceIndex")]
  public Int32? InsuranceIndex { get; set; }

  [Field("InsuranceCounter")]
  public Int32? InsuranceCounter { get; set; }

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
