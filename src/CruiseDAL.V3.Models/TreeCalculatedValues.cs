using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TreeCalculatedValues")]
 public partial class TreeCalculatedValues
 {
  [PrimaryKeyField("TreeCalcValues_CN")]
  public Int32? TreeCalcValues_CN { get; set; }

  [Field("Tree_CN")]
  public Int32 Tree_CN { get; set; }

  [Field("TotalCubicVolume")]
  public Double? TotalCubicVolume { get; set; }

  [Field("GrossBDFTPP")]
  public Double? GrossBDFTPP { get; set; }

  [Field("NetBDFTPP")]
  public Double? NetBDFTPP { get; set; }

  [Field("GrossCUFTPP")]
  public Double? GrossCUFTPP { get; set; }

  [Field("NetCUFTPP")]
  public Double? NetCUFTPP { get; set; }

  [Field("CordsPP")]
  public Double? CordsPP { get; set; }

  [Field("GrossBDFTRemvPP")]
  public Double? GrossBDFTRemvPP { get; set; }

  [Field("GrossCUFTRemvPP")]
  public Double? GrossCUFTRemvPP { get; set; }

  [Field("GrossBDFTSP")]
  public Double? GrossBDFTSP { get; set; }

  [Field("NetBDFTSP")]
  public Double? NetBDFTSP { get; set; }

  [Field("GrossCUFTSP")]
  public Double? GrossCUFTSP { get; set; }

  [Field("NetCUFTSP")]
  public Double? NetCUFTSP { get; set; }

  [Field("CordsSP")]
  public Double? CordsSP { get; set; }

  [Field("GrossCUFTRemvSP")]
  public Double? GrossCUFTRemvSP { get; set; }

  [Field("NumberlogsMS")]
  public Double? NumberlogsMS { get; set; }

  [Field("NumberlogsTPW")]
  public Double? NumberlogsTPW { get; set; }

  [Field("GrossBDFTRP")]
  public Double? GrossBDFTRP { get; set; }

  [Field("GrossCUFTRP")]
  public Double? GrossCUFTRP { get; set; }

  [Field("CordsRP")]
  public Double? CordsRP { get; set; }

  [Field("GrossBDFTIntl")]
  public Double? GrossBDFTIntl { get; set; }

  [Field("NetBDFTIntl")]
  public Double? NetBDFTIntl { get; set; }

  [Field("BiomassMainStemPrimary")]
  public Double? BiomassMainStemPrimary { get; set; }

  [Field("BiomassMainStemSecondary")]
  public Double? BiomassMainStemSecondary { get; set; }

  [Field("ValuePP")]
  public Double? ValuePP { get; set; }

  [Field("ValueSP")]
  public Double? ValueSP { get; set; }

  [Field("ValueRP")]
  public Double? ValueRP { get; set; }

  [Field("BiomassProd")]
  public Double? BiomassProd { get; set; }

  [Field("Biomasstotalstem")]
  public Double? Biomasstotalstem { get; set; }

  [Field("Biomasslivebranches")]
  public Double? Biomasslivebranches { get; set; }

  [Field("Biomassdeadbranches")]
  public Double? Biomassdeadbranches { get; set; }

  [Field("Biomassfoliage")]
  public Double? Biomassfoliage { get; set; }

  [Field("BiomassTip")]
  public Double? BiomassTip { get; set; }

  [Field("TipwoodVolume")]
  public Double? TipwoodVolume { get; set; }

  [Field("ExpansionFactor")]
  public Double? ExpansionFactor { get; set; }

  [Field("TreeFactor")]
  public Double? TreeFactor { get; set; }

  [Field("PointFactor")]
  public Double? PointFactor { get; set; }

 }

}
