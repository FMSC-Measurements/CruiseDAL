using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("SamplerState")]
 public partial class SamplerState
 {
  [PrimaryKeyField("SamplerState_CN")]
  public Int32? SamplerState_CN { get; set; }

  [Field("SampleGroup_CN")]
  public Int32 SampleGroup_CN { get; set; }

  [Field("SampleSelectorType")]
  public String SampleSelectorType { get; set; }

  [Field("BlockState")]
  public String BlockState { get; set; }

  [Field("SystematicIndex")]
  public Int32? SystematicIndex { get; set; }

  [Field("Counter")]
  public Int32? Counter { get; set; }

  [Field("InsuranceIndex")]
  public String InsuranceIndex { get; set; }

  [Field("InsuranceCounter")]
  public String InsuranceCounter { get; set; }

 }

}
