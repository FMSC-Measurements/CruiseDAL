using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("SampleGroup_V3")]
 public partial class SampleGroup_V3
 {
  [PrimaryKeyField("SampleGroup_CN", PersistanceFlags = PersistanceFlags.OnUpdate)]
  public Int32? SampleGroup_CN { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

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
  public Int32? BigBAF { get; set; }

  [Field("TallyBySubPop")]
  public Boolean? TallyBySubPop { get; set; }

  [Field("TallyMethod")]
  public String TallyMethod { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("MinKPI")]
  public Int32? MinKPI { get; set; }

  [Field("MaxKPI")]
  public Int32? MaxKPI { get; set; }

  [Field("SmallFPS")]
  public Double? SmallFPS { get; set; }

 }

}
