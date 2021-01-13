using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("SubPopulation")]
 public partial class SubPopulation
 {
  [PrimaryKeyField("Subpopulation_CN")]
  public Int32? Subpopulation_CN { get; set; }

  [Field("SubPopulationID")]
  public String SubPopulationID { get; set; }

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
