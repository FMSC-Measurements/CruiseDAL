using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("VolumeEquation")]
 public partial class VolumeEquation
 {
  [Field("Species")]
  public String Species { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("VolumeEquationNumber")]
  public String VolumeEquationNumber { get; set; }

  [Field("StumpHeight")]
  public Double StumpHeight { get; set; }

  [Field("TopDIBPrimary")]
  public Double TopDIBPrimary { get; set; }

  [Field("TopDIBSecondary")]
  public Double TopDIBSecondary { get; set; }

  [Field("CalcTotal")]
  public Int64 CalcTotal { get; set; }

  [Field("CalcBoard")]
  public Int64 CalcBoard { get; set; }

  [Field("CalcCubic")]
  public Int64 CalcCubic { get; set; }

  [Field("CalcCord")]
  public Int64 CalcCord { get; set; }

  [Field("CalcTopwood")]
  public Int64 CalcTopwood { get; set; }

  [Field("CalcBiomass")]
  public Int64 CalcBiomass { get; set; }

  [Field("Trim")]
  public Double Trim { get; set; }

  [Field("SegmentationLogic")]
  public Int64 SegmentationLogic { get; set; }

  [Field("MinLogLengthPrimary")]
  public Double MinLogLengthPrimary { get; set; }

  [Field("MaxLogLengthPrimary")]
  public Double MaxLogLengthPrimary { get; set; }

  [Field("MinLogLengthSecondary")]
  public Double MinLogLengthSecondary { get; set; }

  [Field("MaxLogLengthSecondary")]
  public Double MaxLogLengthSecondary { get; set; }

  [Field("MinMerchLength")]
  public Double MinMerchLength { get; set; }

  [Field("Model")]
  public String Model { get; set; }

  [Field("CommonSpeciesName")]
  public String CommonSpeciesName { get; set; }

  [Field("MerchModFlag")]
  public Int64 MerchModFlag { get; set; }

  [Field("EvenOddSegment")]
  public Int64 EvenOddSegment { get; set; }

 }

}