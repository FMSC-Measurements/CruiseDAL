using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [EntitySource("SpeciesCode")]
 public partial class SpeciesCode
 {
  [PrimaryKeyField("Species")]
  public String Species { get; set; }

 }

}
