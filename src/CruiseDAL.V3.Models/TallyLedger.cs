using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TallyLedger")]
 public partial class TallyLedger
 {
  [PrimaryKeyField("TallyLedger_CN")]
  public Int32? TallyLedger_CN { get; set; }

  [Field("TallyLedgerID")]
  public String TallyLedgerID { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("PlotNumber")]
  public Int32? PlotNumber { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("TreeCount")]
  public Int32 TreeCount { get; set; }

  [Field("KPI")]
  public Int32? KPI { get; set; }

  [Field("STM")]
  public Boolean? STM { get; set; }

  [Field("ThreePRandomValue")]
  public Int32? ThreePRandomValue { get; set; }

  [Field("Signature")]
  public String Signature { get; set; }

  [Field("Reason")]
  public String Reason { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

  [Field("EntryType")]
  public String EntryType { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

 }

}
