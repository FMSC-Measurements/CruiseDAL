using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("TreeDefaultValueTreeAuditValue")]
 public partial class TreeDefaultValueTreeAuditValue
 {
  [Field("TreeAuditValue_CN")]
  public Int32 TreeAuditValue_CN { get; set; }

  [Field("TreeDefaultValue_CN")]
  public Int32 TreeDefaultValue_CN { get; set; }

 }

}
