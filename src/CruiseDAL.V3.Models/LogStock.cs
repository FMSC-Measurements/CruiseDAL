using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("LogStock")]
 public partial class LogStock
 {
  [Field("LogStock_CN")]
  public Int64 LogStock_CN { get; set; }

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

  [Field("BoardUtil")]
  public Double BoardUtil { get; set; }

  [Field("CubicUtil")]
  public Double CubicUtil { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

 }

}
