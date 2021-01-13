using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Products")]
 public partial class Products
 {
  [PrimaryKeyField("Products_CN")]
  public Int32? Products_CN { get; set; }

  [Field("Product")]
  public String Product { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

 }

}
