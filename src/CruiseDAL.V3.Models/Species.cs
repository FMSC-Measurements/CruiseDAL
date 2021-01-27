using System;
using FMSC.ORM.EntityModel.Attributes;
namespace CruiseDAL.V3.Models
{
 [Table("Species")]
 public partial class Species
 {
  [PrimaryKeyField("Species_cn")]
  public Int32? Species_cn { get; set; }

  [Field("SpeciesCode")]
  public String SpeciesCode { get; set; }

  [Field("CruiseID")]
  public String CruiseID { get; set; }

  [Field("ContractSpecies")]
  public String ContractSpecies { get; set; }

  [Field("FIACode")]
  public String FIACode { get; set; }

 }

}
