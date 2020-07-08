using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("TreeEstimate")]
 public partial class TreeEstimate
 {
  [PrimaryKeyField("TreeEstimate_CN")]
  public Int32? TreeEstimate_CN { get; set; }

  [Field("CountTree_CN")]
  public Int32? CountTree_CN { get; set; }

  [Field("TreeEstimate_GUID")]
  public String TreeEstimate_GUID { get; set; }

  [Field("KPI")]
  public Double KPI { get; set; }

 }

}
