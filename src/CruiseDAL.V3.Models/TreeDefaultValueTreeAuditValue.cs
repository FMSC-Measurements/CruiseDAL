using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TreeDefaultValueTreeAuditValue")]
 public partial class TreeDefaultValueTreeAuditValue
 {
  [Field("TreeDefaultValue_CN")]
  public Int64 TreeDefaultValue_CN { get; set; }

  [Field("TreeAuditValue_CN")]
  public Int64 TreeAuditValue_CN { get; set; }

 }

}
