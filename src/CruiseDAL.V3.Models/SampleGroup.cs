using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("SampleGroup")]
 public partial class SampleGroup
 {
  [Field("SampleGroup_CN")]
  public Int64 SampleGroup_CN { get; set; }

  [Field("Code")]
  public String Code { get; set; }

  [Field("Stratum_CN")]
  public Int64 Stratum_CN { get; set; }

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
  public Int64 SamplingFrequency { get; set; }

  [Field("InsuranceFrequency")]
  public Int64 InsuranceFrequency { get; set; }

  [Field("KZ")]
  public Int64 KZ { get; set; }

  [Field("BigBAF")]
  public Int64 BigBAF { get; set; }

  [Field("TallyBySubPop")]
  public Boolean TallyBySubPop { get; set; }

  [Field("TallyMethod")]
  public String TallyMethod { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("SampleSelectorType")]
  public String SampleSelectorType { get; set; }

  [Field("SampleSelectorState")]
  public String SampleSelectorState { get; set; }

  [Field("MinKPI")]
  public Int64 MinKPI { get; set; }

  [Field("MaxKPI")]
  public Int64 MaxKPI { get; set; }

  [Field("SmallFPS")]
  public Double SmallFPS { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Int64 RowVersion { get; set; }

 }

}
