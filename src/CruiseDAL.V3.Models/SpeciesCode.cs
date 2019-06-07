using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("SpeciesCode")]
 public partial class SpeciesCode
 {
  [Field("Species")]
  public Object Species { get; set; }

 }

}
