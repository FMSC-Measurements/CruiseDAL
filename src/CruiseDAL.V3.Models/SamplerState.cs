using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("SamplerState")]
 public partial class SamplerState
 {
  [PrimaryKeyField("SamplerState_CN")]
  public Int32? SamplerState_CN { get; set; }

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
