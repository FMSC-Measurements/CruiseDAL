using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("SamplerState")]
 public partial class SamplerState
 {
  [Field("SamplerState_CN")]
  public Int64 SamplerState_CN { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("SampleSelectorType")]
  public String SampleSelectorType { get; set; }

  [Field("SampleSelectorState")]
  public String SampleSelectorState { get; set; }

 }

}
