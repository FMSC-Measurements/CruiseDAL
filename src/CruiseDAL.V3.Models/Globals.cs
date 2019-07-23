using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Globals")]
 public partial class Globals
 {
  [Field("Block")]
  public String Block { get; set; }

  [Field("Key")]
  public String Key { get; set; }

  [Field("Value")]
  public String Value { get; set; }

 }

}
