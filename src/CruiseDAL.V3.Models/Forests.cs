using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Forests")]
 public partial class Forests
 {
  [PrimaryKeyField("Forests_CN")]
  public Int32? Forests_CN { get; set; }

  [Field("Forest")]
  public String Forest { get; set; }

  [Field("Region")]
  public String Region { get; set; }

  [Field("FriendlyName")]
  public String FriendlyName { get; set; }

  [Field("State")]
  public String State { get; set; }

 }

}
