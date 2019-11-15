using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Log")]
 public partial class Log
 {
  [PrimaryKeyField("Log_CN")]
  public Int32? Log_CN { get; set; }

  [Field("Log_GUID")]
  public String Log_GUID { get; set; }

  [Field("Tree_CN")]
  public Int32 Tree_CN { get; set; }

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

 }

}
