using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeField")]
 public partial class TreeField
 {
  [Field("TreeField_CN")]
  public Int64 TreeField_CN { get; set; }

  [Field("Field")]
  public String Field { get; set; }

  [Field("DbType")]
  public String DbType { get; set; }

  [Field("IsTreeMeasurmentField")]
  public Boolean IsTreeMeasurmentField { get; set; }

 }

}
