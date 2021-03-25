using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Stratum_Tombstone")]
 public partial class Stratum_Tombstone
 {
  [Field("StratumID")]
  public String StratumID { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("Description")]
  public String Description { get; set; }

  [Field("Method")]
  public String Method { get; set; }

  [Field("BasalAreaFactor")]
  public Double? BasalAreaFactor { get; set; }

  [Field("FixedPlotSize")]
  public Double? FixedPlotSize { get; set; }

  [Field("KZ3PPNT")]
  public Int32? KZ3PPNT { get; set; }

  [Field("SamplingFrequency")]
  public Int32? SamplingFrequency { get; set; }

  [Field("Hotkey")]
  public String Hotkey { get; set; }

  [Field("FBSCode")]
  public String FBSCode { get; set; }

  [Field("YieldComponent")]
  public String YieldComponent { get; set; }

  [Field("Month")]
  public Int32? Month { get; set; }

  [Field("Year")]
  public Int32? Year { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("Created_TS")]
  public DateTime? Created_TS { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("Modified_TS")]
  public DateTime? Modified_TS { get; set; }

  [Field("Deleted_TS")]
  public DateTime? Deleted_TS { get; set; }

 }

}
