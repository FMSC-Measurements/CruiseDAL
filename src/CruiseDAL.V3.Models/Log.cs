using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Log")]
 public partial class Log
 {
  [Field("Log_CN")]
  public Int64 Log_CN { get; set; }

  [Field("Log_GUID")]
  public String Log_GUID { get; set; }

  [Field("Tree_CN")]
  public Int64 Tree_CN { get; set; }

  [Field("LogNumber")]
  public String LogNumber { get; set; }

  [Field("Grade")]
  public String Grade { get; set; }

  [Field("SeenDefect")]
  public Double SeenDefect { get; set; }

  [Field("PercentRecoverable")]
  public Double PercentRecoverable { get; set; }

  [Field("Length")]
  public Int64 Length { get; set; }

  [Field("ExportGrade")]
  public String ExportGrade { get; set; }

  [Field("SmallEndDiameter")]
  public Double SmallEndDiameter { get; set; }

  [Field("LargeEndDiameter")]
  public Double LargeEndDiameter { get; set; }

  [Field("GrossBoardFoot")]
  public Double GrossBoardFoot { get; set; }

  [Field("NetBoardFoot")]
  public Double NetBoardFoot { get; set; }

  [Field("GrossCubicFoot")]
  public Double GrossCubicFoot { get; set; }

  [Field("NetCubicFoot")]
  public Double NetCubicFoot { get; set; }

  [Field("BoardFootRemoved")]
  public Double BoardFootRemoved { get; set; }

  [Field("CubicFootRemoved")]
  public Double CubicFootRemoved { get; set; }

  [Field("DIBClass")]
  public Double DIBClass { get; set; }

  [Field("BarkThickness")]
  public Double BarkThickness { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Int64 RowVersion { get; set; }

 }

}
