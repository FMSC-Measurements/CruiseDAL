using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Stem_V3")]
 public partial class Stem_V3
 {
  [PrimaryKeyField("Stem_CN", PersistanceFlags = PersistanceFlags.OnUpdate)]
  public Int32? Stem_CN { get; set; }

  [Field("StemID")]
  public String StemID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Diameter")]
  public Double? Diameter { get; set; }

  [Field("DiameterType")]
  public String DiameterType { get; set; }

 }

}
