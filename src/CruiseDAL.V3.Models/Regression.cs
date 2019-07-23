using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Regression")]
 public partial class Regression
 {
  [PrimaryKeyField("Regression_CN")]
  public Int32? Regression_CN { get; set; }

  [Field("rVolume")]
  public String rVolume { get; set; }

  [Field("rVolType")]
  public String rVolType { get; set; }

  [Field("rSpeices")]
  public String rSpeices { get; set; }

  [Field("rProduct")]
  public String rProduct { get; set; }

  [Field("rLiveDead")]
  public String rLiveDead { get; set; }

  [Field("CoefficientA")]
  public Double? CoefficientA { get; set; }

  [Field("CoefficientB")]
  public Double? CoefficientB { get; set; }

  [Field("CoefficientC")]
  public Double? CoefficientC { get; set; }

  [Field("TotalTrees")]
  public Int32? TotalTrees { get; set; }

  [Field("MeanSE")]
  public Double? MeanSE { get; set; }

  [Field("Rsquared")]
  public Double? Rsquared { get; set; }

  [Field("RegressModel")]
  public String RegressModel { get; set; }

  [Field("rMinDbh")]
  public Double? rMinDbh { get; set; }

  [Field("rMaxDbh")]
  public Double? rMaxDbh { get; set; }

 }

}
