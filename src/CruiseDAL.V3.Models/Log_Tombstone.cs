using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Log_Tombstone")]
 public partial class Log_Tombstone
 {
  [Field("LogID")]
  public String LogID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("LogNumber")]
  public String LogNumber { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

  [Field("SeenDefect")]
  public Double? SeenDefect { get; set; }

  [Field("PercentRecoverable")]
  public Double? PercentRecoverable { get; set; }

  [Field("Length")]
  public Int32? Length { get; set; }

  [Field("ExportGrade")]
  public String ExportGrade { get; set; }

  [Field("SmallEndDiameter")]
  public Double? SmallEndDiameter { get; set; }

  [Field("LargeEndDiameter")]
  public Double? LargeEndDiameter { get; set; }

  [Field("GrossBoardFoot")]
  public Double? GrossBoardFoot { get; set; }

  [Field("NetBoardFoot")]
  public Double? NetBoardFoot { get; set; }

  [Field("GrossCubicFoot")]
  public Double? GrossCubicFoot { get; set; }

  [Field("NetCubicFoot")]
  public Double? NetCubicFoot { get; set; }

  [Field("BoardFootRemoved")]
  public Double? BoardFootRemoved { get; set; }

  [Field("CubicFootRemoved")]
  public Double? CubicFootRemoved { get; set; }

  [Field("DIBClass")]
  public Double? DIBClass { get; set; }

  [Field("BarkThickness")]
  public Double? BarkThickness { get; set; }

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
