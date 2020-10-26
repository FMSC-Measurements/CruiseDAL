using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("FixCNTTallyPopulation")]
 public partial class FixCNTTallyPopulation
 {
  [PrimaryKeyField("FixCNTTallyPopulation_CN")]
  public Int32? FixCNTTallyPopulation_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("IntervalSize")]
  public Int32? IntervalSize { get; set; }

  [Field("Min")]
  public Int32? Min { get; set; }

  [Field("Max")]
  public Int32? Max { get; set; }

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
