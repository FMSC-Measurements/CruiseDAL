using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("TallyHotKey")]
 public partial class TallyHotKey
 {
  [PrimaryKeyField("TallyHotKey_CN")]
  public Int32? TallyHotKey_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("SampleGroupCode")]
  public String SampleGroupCode { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("LiveDead")]
  public String LiveDead { get; set; }

  [Field("HotKey")]
  public String HotKey { get; set; }

 }

}
