using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("VolumeEquation")]
 public partial class VolumeEquation
 {
  [Field("Species")]
  public String Species { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("VolumeEquationNumber")]
  public String VolumeEquationNumber { get; set; }

  [Field("StumpHeight")]
  public Double? StumpHeight { get; set; }

  [Field("TopDIBPrimary")]
  public Double? TopDIBPrimary { get; set; }

  [Field("TopDIBSecondary")]
  public Double? TopDIBSecondary { get; set; }

  [Field("CalcTotal")]
  public Int32? CalcTotal { get; set; }

  [Field("CalcBoard")]
  public Int32? CalcBoard { get; set; }

  [Field("CalcCubic")]
  public Int32? CalcCubic { get; set; }

  [Field("CalcCord")]
  public Int32? CalcCord { get; set; }

  [Field("CalcTopwood")]
  public Int32? CalcTopwood { get; set; }

  [Field("CalcBiomass")]
  public Int32? CalcBiomass { get; set; }

  [Field("Trim")]
  public Double? Trim { get; set; }

  [Field("SegmentationLogic")]
  public Int32? SegmentationLogic { get; set; }

  [Field("MinLogLengthPrimary")]
  public Double? MinLogLengthPrimary { get; set; }

  [Field("MaxLogLengthPrimary")]
  public Double? MaxLogLengthPrimary { get; set; }

  [Field("MinMerchLength")]
  public Double? MinMerchLength { get; set; }

  [Field("Model")]
  public String Model { get; set; }

  [Field("CommonSpeciesName")]
  public String CommonSpeciesName { get; set; }

  [Field("MerchModFlag")]
  public Int32? MerchModFlag { get; set; }

  [Field("EvenOddSegment")]
  public Int32? EvenOddSegment { get; set; }

 }

}