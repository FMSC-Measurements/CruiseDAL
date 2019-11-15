using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V2.Models
{
 [Table("Stem")]
 public partial class Stem
 {
  [PrimaryKeyField("Stem_CN")]
  public Int32? Stem_CN { get; set; }

  [Field("Stem_GUID")]
  public String Stem_GUID { get; set; }

  [Field("Tree_CN")]
  public Int32? Tree_CN { get; set; }

  [Field("Diameter")]
  public Double? Diameter { get; set; }

  [Field("DiameterType")]
  public String DiameterType { get; set; }

 }

}
