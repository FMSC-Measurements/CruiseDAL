using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("TallyHotKey")]
 public partial class TallyHotKey
 {
  [Field("TallyHotKey_CN")]
  public Int64 TallyHotKey_CN { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("HotKey")]
  public String HotKey { get; set; }

 }

}
