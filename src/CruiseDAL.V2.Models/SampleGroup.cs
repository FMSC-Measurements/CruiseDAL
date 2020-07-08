using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("SampleGroup")]
 public partial class SampleGroup
 {
  [PrimaryKeyField("SampleGroup_CN")]
  public Int32? SampleGroup_CN { get; set; }

  [Field("Stratum_CN")]
  public Int32 Stratum_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("CutLeave")]
  public String CutLeave { get; set; }

  [Field("UOM")]
  public String UOM { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("SecondaryProduct")]
  public String SecondaryProduct { get; set; }

  [Field("BiomassProduct")]
  public String BiomassProduct { get; set; }

  [Field("DefaultLiveDead")]
  public String DefaultLiveDead { get; set; }

  [Field("SamplingFrequency")]
  public Int32? SamplingFrequency { get; set; }

  [Field("InsuranceFrequency")]
  public Int32? InsuranceFrequency { get; set; }

  [Field("KZ")]
  public Int32? KZ { get; set; }

  [Field("BigBAF")]
  public Double? BigBAF { get; set; }

  [Field("SmallFPS")]
  public Double? SmallFPS { get; set; }

  [Field("TallyMethod")]
  public String TallyMethod { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("SampleSelectorType")]
  public String SampleSelectorType { get; set; }

  [Field("SampleSelectorState")]
  public String SampleSelectorState { get; set; }

  [Field("MinKPI")]
  public Int32? MinKPI { get; set; }

  [Field("MaxKPI")]
  public Int32? MaxKPI { get; set; }

 }

}
