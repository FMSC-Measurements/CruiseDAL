using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("BiomassEquation")]
 public partial class BiomassEquation
 {
  [Field("Species")]
  public String Species { get; set; }

  [Field("Product")]
  public String Product { get; set; }

  [Field("Component")]
  public String Component { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("FIAcode")]
  public Int32 FIAcode { get; set; }

  [Field("Equation")]
  public String Equation { get; set; }

  [Field("PercentMoisture")]
  public Double? PercentMoisture { get; set; }

  [Field("PercentRemoved")]
  public Double? PercentRemoved { get; set; }

  [Field("MetaData")]
  public String MetaData { get; set; }

  [Field("WeightFactorPrimary")]
  public Double? WeightFactorPrimary { get; set; }

  [Field("WeightFactorSecondary")]
  public Double? WeightFactorSecondary { get; set; }

 }

}
