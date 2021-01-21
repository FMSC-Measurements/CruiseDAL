using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_Purpose")]
 public partial class LK_Purpose
 {
  [PrimaryKeyField("LK_Purpose_CN")]
  public Int32? LK_Purpose_CN { get; set; }

  [Field("Purpose")]
  public String Purpose { get; set; }

  [Field("ShortCode")]
  public String ShortCode { get; set; }

 }

}
