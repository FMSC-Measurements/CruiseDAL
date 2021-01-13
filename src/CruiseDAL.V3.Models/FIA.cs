using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("FIA")]
 public partial class FIA
 {
  [PrimaryKeyField("FIA_cn")]
  public Int32? FIA_cn { get; set; }

  [Field("FIACode")]
  public Int32 FIACode { get; set; }

  [Field("CommonName")]
  public String CommonName { get; set; }

 }

}
