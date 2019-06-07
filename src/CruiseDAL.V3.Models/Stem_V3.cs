using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("Stem_V3")]
 public partial class Stem_V3
 {
  [Field("Stem_CN")]
  public Int64 Stem_CN { get; set; }

  [Field("StemID")]
  public String StemID { get; set; }

  [Field("TreeID")]
  public String TreeID { get; set; }

  [Field("Diameter")]
  public Double Diameter { get; set; }

  [Field("DiameterType")]
  public String DiameterType { get; set; }

  [Field("CreatedBy")]
  public String CreatedBy { get; set; }

  [Field("CreatedDate")]
  public DateTime CreatedDate { get; set; }

  [Field("ModifiedBy")]
  public String ModifiedBy { get; set; }

  [Field("ModifiedDate")]
  public DateTime ModifiedDate { get; set; }

  [Field("RowVersion")]
  public Int64 RowVersion { get; set; }

 }

}
