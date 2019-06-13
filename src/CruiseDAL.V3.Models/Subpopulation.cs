using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Subpopulation")]
 public partial class Subpopulation
 {
  [PrimaryKeyField("Subpopulation_CN", PersistanceFlags = PersistanceFlags.OnUpdate)]
  public Int32? Subpopulation_CN { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("Species")]
  public String Species { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

 }

}
