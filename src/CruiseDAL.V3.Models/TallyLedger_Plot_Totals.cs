using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TallyLedger_Plot_Totals")]
 public partial class TallyLedger_Plot_Totals
 {
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

  [Field("PlotNumber")]
  public Int32? PlotNumber { get; set; }

  [Field("TreeCount")]
  public String TreeCount { get; set; }

  [Field("KPI")]
  public String KPI { get; set; }

 }

}
