using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("SampleGroup")]
 public partial class SampleGroup
 {
  [PrimaryKeyField("SampleGroup_CN")]
  public Int32? SampleGroup_CN { get; set; }

  [Field("SampleGroupID")]
  public String SampleGroupID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

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

  [Field("SampleSelectorType")]
  public String SampleSelectorType { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("MinKPI")]
  public Int32? MinKPI { get; set; }

  [Field("MaxKPI")]
  public Int32? MaxKPI { get; set; }

  [Field("SmallFPS")]
  public Double? SmallFPS { get; set; }

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
