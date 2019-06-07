using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("FixCNTTallyClass_V3")]
 public partial class FixCNTTallyClass_V3
 {
  [Field("FixCNTTallyClass_CN")]
  public Int64 FixCNTTallyClass_CN { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("Field")]
  public String Field { get; set; }

 }

}
