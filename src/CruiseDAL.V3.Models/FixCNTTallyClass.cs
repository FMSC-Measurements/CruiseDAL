using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("FixCNTTallyClass")]
 public partial class FixCNTTallyClass
 {
  [PrimaryKeyField("FixCNTTallyClass_CN")]
  public Int32? FixCNTTallyClass_CN { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("StratumCode")]
  public String StratumCode { get; set; }

  [Field("Field")]
  public String Field { get; set; }

 }

}
