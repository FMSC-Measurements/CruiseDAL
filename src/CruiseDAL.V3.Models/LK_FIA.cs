using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_FIA")]
 public partial class LK_FIA
 {
  [PrimaryKeyField("LK_FIA_CN")]
  public Int32? LK_FIA_CN { get; set; }

  [Field("FIACode")]
  public Int32 FIACode { get; set; }

  [Field("CommonName")]
  public String CommonName { get; set; }

 }

}
