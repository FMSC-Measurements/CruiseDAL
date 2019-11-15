using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Tree_V3")]
 public partial class Tree_V3
 {
  [PrimaryKeyField("Tree_CN")]
  public Int32? Tree_CN { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("CuttingUnitCode")]
  public String CuttingUnitCode { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("PlotNumber")]
  public Int32? PlotNumber { get; set; }

  [Field("TreeNumber")]
  public Int32 TreeNumber { get; set; }

  [Field("CountOrMeasure")]
  public String CountOrMeasure { get; set; }

  [Field("XCoordinate")]
  public Double? XCoordinate { get; set; }

  [Field("YCoordinate")]
  public Double? YCoordinate { get; set; }

  [Field("ZCoordinate")]
  public Double? ZCoordinate { get; set; }

 }

}
