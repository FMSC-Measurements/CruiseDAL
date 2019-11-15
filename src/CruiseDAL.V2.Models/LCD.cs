using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("LCD")]
 public partial class LCD
 {
  [PrimaryKeyField("LCD_CN")]
  public Int32? LCD_CN { get; set; }

  [Field("CutLeave")]
  public String CutLeave { get; set; }

  [Field("Stratum")]
  public String Stratum { get; set; }

  [Field("SampleGroup")]
  public String SampleGroup { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("PrimaryProduct")]
  public String PrimaryProduct { get; set; }

  [Field("SecondaryProduct")]
  public String SecondaryProduct { get; set; }

  [Field("UOM")]
  public String UOM { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("Yield")]
  public String Yield { get; set; }

  [Field("ContractSpecies")]
  public String ContractSpecies { get; set; }

  [Field("TreeGrade")]
  public String TreeGrade { get; set; }

  [Field("STM")]
  public String STM { get; set; }

  [Field("FirstStageTrees")]
  public Double? FirstStageTrees { get; set; }

  [Field("MeasuredTrees")]
  public Double? MeasuredTrees { get; set; }

  [Field("TalliedTrees")]
  public Double? TalliedTrees { get; set; }

  [Field("SumKPI")]
  public Double? SumKPI { get; set; }

  [Field("SumMeasuredKPI")]
  public Double? SumMeasuredKPI { get; set; }

  [Field("SumExpanFactor")]
  public Double? SumExpanFactor { get; set; }

  [Field("SumDBHOB")]
  public Double? SumDBHOB { get; set; }

  [Field("SumDBHOBsqrd")]
  public Double? SumDBHOBsqrd { get; set; }

  [Field("SumTotHgt")]
  public Double? SumTotHgt { get; set; }

  [Field("SumHgtUpStem")]
  public Double? SumHgtUpStem { get; set; }

  [Field("SumMerchHgtPrim")]
  public Double? SumMerchHgtPrim { get; set; }

  [Field("SumMerchHgtSecond")]
  public Double? SumMerchHgtSecond { get; set; }

  [Field("SumLogsMS")]
  public Double? SumLogsMS { get; set; }

  [Field("SumTotCubic")]
  public Double? SumTotCubic { get; set; }

  [Field("SumGBDFT")]
  public Double? SumGBDFT { get; set; }

  [Field("SumNBDFT")]
  public Double? SumNBDFT { get; set; }

  [Field("SumGCUFT")]
  public Double? SumGCUFT { get; set; }

  [Field("SumNCUFT")]
  public Double? SumNCUFT { get; set; }

  [Field("SumGBDFTremv")]
  public Double? SumGBDFTremv { get; set; }

  [Field("SumGCUFTremv")]
  public Double? SumGCUFTremv { get; set; }

  [Field("SumCords")]
  public Double? SumCords { get; set; }

  [Field("SumWgtMSP")]
  public Double? SumWgtMSP { get; set; }

  [Field("SumValue")]
  public Double? SumValue { get; set; }

  [Field("SumGBDFTtop")]
  public Double? SumGBDFTtop { get; set; }

  [Field("SumNBDFTtop")]
  public Double? SumNBDFTtop { get; set; }

  [Field("SumGCUFTtop")]
  public Double? SumGCUFTtop { get; set; }

  [Field("SumNCUFTtop")]
  public Double? SumNCUFTtop { get; set; }

  [Field("SumCordsTop")]
  public Double? SumCordsTop { get; set; }

  [Field("SumWgtMSS")]
  public Double? SumWgtMSS { get; set; }

  [Field("SumTopValue")]
  public Double? SumTopValue { get; set; }

  [Field("SumLogsTop")]
  public Double? SumLogsTop { get; set; }

  [Field("SumBDFTrecv")]
  public Double? SumBDFTrecv { get; set; }

  [Field("SumCUFTrecv")]
  public Double? SumCUFTrecv { get; set; }

  [Field("SumCordsRecv")]
  public Double? SumCordsRecv { get; set; }

  [Field("SumValueRecv")]
  public Double? SumValueRecv { get; set; }

  [Field("BiomassProduct")]
  public Double? BiomassProduct { get; set; }

  [Field("SumWgtBAT")]
  public Double? SumWgtBAT { get; set; }

  [Field("SumWgtBBL")]
  public Double? SumWgtBBL { get; set; }

  [Field("SumWgtBBD")]
  public Double? SumWgtBBD { get; set; }

  [Field("SumWgtBFT")]
  public Double? SumWgtBFT { get; set; }

  [Field("SumWgtTip")]
  public Double? SumWgtTip { get; set; }

  [Field("SumTipwood")]
  public Double? SumTipwood { get; set; }

 }

}
