using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("CountTree")]
 public partial class CountTree
 {
  [Field("CountTree_CN")]
  public Object CountTree_CN { get; set; }

  [Field("CuttingUnit_CN")]
  public Int64 CuttingUnit_CN { get; set; }

  [Field("SampleGroup_CN")]
  public Int64 SampleGroup_CN { get; set; }

  [Field("TreeDefaultValue_CN")]
  public Int64 TreeDefaultValue_CN { get; set; }

  [Field("Tally_CN")]
  public Object Tally_CN { get; set; }

  [Field("Component_CN")]
  public Object Component_CN { get; set; }

  [Field("TreeCount")]
  public Object TreeCount { get; set; }

  [Field("SumKPI")]
  public Object SumKPI { get; set; }

  [Field("CreatedBy")]
  public Object CreatedBy { get; set; }

  [Field("CreatedDate")]
  public Object CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public Object ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public Object ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Object RowVersion { get; set; }

 }

}
