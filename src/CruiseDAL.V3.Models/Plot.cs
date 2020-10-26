using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Plot")]
 public partial class Plot
 {
  [PrimaryKeyField("Plot_CN")]
  public Int32? Plot_CN { get; set; }

  [Field("PlotID")]
  public String PlotID { get; set; }

  [Field("PlotNumber")]
  public Int32 PlotNumber { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("Slope")]
  public Double? Slope { get; set; }

  [Field("Aspect")]
  public Double? Aspect { get; set; }

  [Field("Remarks")]
  public String Remarks { get; set; }

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
