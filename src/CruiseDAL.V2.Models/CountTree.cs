using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("CountTree")]
 public partial class CountTree
 {
  [PrimaryKeyField("CountTree_CN")]
  public Int32? CountTree_CN { get; set; }

  [Field("SampleGroup_CN")]
  public Int32 SampleGroup_CN { get; set; }

  [Field("CuttingUnit_CN")]
  public Int32 CuttingUnit_CN { get; set; }

  [Field("Tally_CN")]
  public Int32? Tally_CN { get; set; }

  [Field("TreeDefaultValue_CN")]
  public Int32? TreeDefaultValue_CN { get; set; }

  [Field("Component_CN")]
  public Int32? Component_CN { get; set; }

  [Field("TreeCount")]
  public Int32? TreeCount { get; set; }

  [Field("SumKPI")]
  public Int32? SumKPI { get; set; }

 }

}
