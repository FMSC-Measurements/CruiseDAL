using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("LK_Product")]
 public partial class LK_Product
 {
  [PrimaryKeyField("LK_Product_CN")]
  public Int32? LK_Product_CN { get; set; }

  [Field("Product")]
  public String Product { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
