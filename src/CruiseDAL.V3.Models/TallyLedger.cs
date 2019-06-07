using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TallyLedger")]
 public partial class TallyLedger
 {
  [Field("TallyLedger_CN")]
  public Int64 TallyLedger_CN { get; set; }

  [Field("TallyLedgerID")]
  public String TallyLedgerID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("PlotNumber")]
  public Int64 PlotNumber { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("TreeCount")]
  public Int64 TreeCount { get; set; }

  [Field("KPI")]
  public Int64 KPI { get; set; }

  [Field("STM")]
  public Boolean STM { get; set; }

  [Field("ThreePRandomValue")]
  public Int64 ThreePRandomValue { get; set; }

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

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("IsDeleted")]
  public Boolean IsDeleted { get; set; }

 }

}
