using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeEstimate")]
 public partial class TreeEstimate
 {
  [Field("TreeEstimate_CN")]
  public Object TreeEstimate_CN { get; set; }

  [Field("CountTree_CN")]
  public Object CountTree_CN { get; set; }

  [Field("KPI")]
  public Object KPI { get; set; }

  [Field("CreatedBy")]
  public Object CreatedBy { get; set; }

  [Field("CreatedDate")]
  public Object CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public Object ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public Object ModifiedDate { get; set; }

 }

}
